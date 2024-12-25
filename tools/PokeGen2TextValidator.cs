using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace PokeGen2TextValidator
{
    internal class PokeGen2TextValidator
    {
        // Program.cs
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelpText();
                return 0;
            }

            bool validate = true;
            bool compare = false;
            bool merge = false;

            int currentArg = 0;
            if (args[currentArg].ToLowerInvariant().StartsWith("-") && args.Length > 1)
            {
                switch (args[currentArg])
                {
                    case "-v":
                    case "--validate":
                    {
                        validate = true;
                        compare = false;
                        merge = false;
                        break;
                    }
                    case "-c":
                    case "--compare":
                    {
                        validate = false;
                        compare = true;
                        merge = false;
                        break;
                    }
                    case "-m":
                    case "--merge":
                    {
                        validate = false;
                        compare = false;
                        merge = true;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Unknown option: " + args[currentArg][1]);
                        PrintHelpText();
                        return 1;
                    }
                }

                ++currentArg;
            }

            ASMFile source = GetASMFile(args[currentArg++]);
            if (source == null || source.Type == FileType.Ignore)
            {
                return 0;
            }

            if (validate)
            {
                Console.WriteLine("Validating file: " + source.File.Name + " Type: " + source.Type + " Source: " + source.Source);

                bool problem = false;
                foreach (KeyValuePair<string, Block> pair in source.blocks)
                {
                    Validator validator = new Validator(pair.Value);
                    string message = validator.Validate();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        if (message.Contains("Error"))
                        {
                            problem = true;
                        }
                        Console.Write(message);
                    }
                }

                Console.WriteLine("Result: " + (problem ? "Failed!\n" : "Passed!\n"));
                return problem ? 1 : 0;
            }
            else if (compare || merge)
            {
                ASMFile target = GetASMFile(args[currentArg++]);

                if (compare)
                {
                    Comparer comparer = new Comparer(source, target);
                    Console.WriteLine(comparer.Compare());
                }
                else if (merge)
                {
                    ASMFile baseSource = GetASMFile(args[currentArg++]);
                    ASMFile baseTarget = GetASMFile(args[currentArg++]);

                    if (source != null && baseSource != null && baseTarget != null)
                    {
                        Merger merger = new Merger(source, target, baseSource, baseTarget);
                        Console.WriteLine(merger.Merge());
                        File.WriteAllText(target.File.FullName, target.ToString());
                    }
                }
            }

            return 0;
        }

        private static void PrintHelpText()
        {
            Console.WriteLine("Usage: PokeGen2TextValidator [-vcm] Source [Target] [BaseSource] [BaseTarget]");
            Console.WriteLine("  Options:");
            Console.WriteLine("    -v, --validate: Validate Source. Default behavior.");
            Console.WriteLine("    -c, --compare:  Compare Source to Target.");
            Console.WriteLine("    -m, --merge:    Compare BaseSource to BaseTarget, then merge matching blocks from Source into Target.");
            Console.WriteLine("  Source: Primary file to operate on.");
            Console.WriteLine("  Target: Target file to compare to Source. Required for compares and merges.");
            Console.WriteLine("  BaseSource: Original version of Source to compare to BaseTarget to see if merge from Source to Target should occur.");
            Console.WriteLine("  BaseTarget: Original version of Target to compare to BaseSource to see if merge from Source to Target should occur.");
        }

        private static ASMFile GetASMFile(string path)
        {
            path = path.Replace("\\", "/");
            if (!path.EndsWith(".asm"))
            {
                Console.WriteLine("File " + path + " is not an asm file!");
                return null;
            }
            if (!File.Exists(path))
            {
                Console.WriteLine("File " + path + " not found!");
                return null;
            }
            return new ASMFile(path);
        }
    }

    // ASMFile.cs
    public enum FileType
    {
        /// <summary> Invalid file, skip it. Generally these are files still containing unused Japanese text. </summary>
        Ignore = -1,
        /// <summary> Unknown or miscellaneous data. No string validation will be available. </summary>
        Misc = 0,
        /// <summary> Text for a textbox. Limit 18 chars per line of text. </summary>
        TextBox = 1,
        /// <summary> Pokédex entry. </summary>
        Pokedex = 2,
        /// <summary> Pokémon names. </summary>
        Pokemon = 3,
        /// <summary> Move names. </summary>
        Move = 4,
        /// <summary> Item names. </summary>
        Item = 5,
        /// <summary> Type names. </summary>
        Type = 6,
        /// <summary> Map landmarks. Town, route, dungeon names. </summary>
        Landmark = 7,
        /// <summary> Trainer class names. </summary>
        TrainerClass = 8,
    }

    public enum Source
    {
        Unknown = -1,
        GoldSilver = 0,
        Crystal = 1
    }

    internal class ASMFile : IEnumerable<Block>
    {

        public string Name { get; private set; }
        public FileInfo File { get; private set; }
        public FileType Type { get; private set; }
        public Source Source { get; private set; }
        public Dictionary<string, Block> blocks;

        public ASMFile(string path)
        {
            File = new FileInfo(path);
            Name = File.Name;
            Type = GetType(File);
            blocks = new Dictionary<string, Block>();

            Source = Source.Unknown;
            if (File.FullName.Contains("pokegold"))
            {
                Source = Source.GoldSilver;
            }
            else if (File.FullName.Contains("pokecrystal"))
            {
                Source = Source.Crystal;
            }

            string[] lines = System.IO.File.ReadAllLines(path);
            ParseBlocks(lines);
        }

        public void Add(Block block)
        {
            blocks.Add(block.Name, block);
        }

        public static FileType GetType(FileInfo fileInfo)
        {
            string path = fileInfo.FullName;
            path = path.Replace('\\', '/');

            if (path.Contains("data/pokemon/dex_entries"))
            {
                return FileType.Pokedex;
            }
            else if (path.Contains("data/pokemon/names.asm"))
            {
                return FileType.Pokemon;
            }
            else if (path.Contains("data/moves/names.asm"))
            {
                return FileType.Move;
            }
            else if (path.Contains("data/items/names.asm"))
            {
                return FileType.Item;
            }
            else if (path.Contains("data/types/names.asm")
                || path.Contains("data/types/search_strings.asm")
                )
            {
                return FileType.Type;
            }
            else if (path.Contains("data/maps/landmarks.asm"))
            {
                return FileType.Landmark;
            }
            else if (path.Contains("data/trainers/class_names.asm"))
            {
                return FileType.TrainerClass;
            }
            else if (path.EndsWith("data/battle_tower/trainer_text.asm")
                || path.EndsWith("data/items/descriptions.asm")
                || path.EndsWith("data/moves/descriptions.asm")
                || path.EndsWith("data/text/battle.asm")
                || path.EndsWith("data/text/common_1.asm")
                || path.EndsWith("data/text/common_2.asm")
                || path.EndsWith("data/text/common_3.asm")
                || path.EndsWith("data/text/std_text.asm")
                || path.EndsWith("data/text/unused_sweet_honey.asm")
                || path.Contains("data/phone/text/")
                || path.EndsWith("maps/" + fileInfo.Name)
                )
            {
                return FileType.TextBox;
            }
            else if (path.Contains("mobile/"))
            {
                return FileType.Ignore;
            }

            return FileType.Misc;
        }

        private void ParseBlocks(string[] lines)
        {
            Block currentBlock = new Block("default", Type, Source);
            for (int i = 0; i < lines.Length; ++i)
            {
                Line line = new Line(i, lines[i]);

                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    if (!line.Text.StartsWith(".") && line.Text.EndsWith(":"))
                    {
                        blocks[currentBlock.Name] = currentBlock;
                        currentBlock = new Block(line.Text, Type, Source);
                    }
                }
                currentBlock.Add(line);
            }

            blocks[currentBlock.Name] = currentBlock;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block block in blocks.Values)
            {
                foreach (Line line in block.lines)
                {
                    sb.Append(line.Raw).Append('\n');
                }
            }

            return sb.ToString();
        }

        public IEnumerator<Block> GetEnumerator()
        {
            foreach (KeyValuePair<string, Block> pair in blocks)
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<string, Block> pair in blocks)
            {
                yield return pair.Value;
            }
        }
    }

    // Block.cs
    internal class Block
    {
        public List<Line> lines;
        public FileType Type { get; set; }
        public Source Source { get; set; }
        public string Name { get; private set; }

        /// <summary>
        /// Finds unconditional replacement annotations and returns the name of the source block to use.
        /// </summary>
        /// <returns>Name of block in source to use. <c>null</c> if unspecified.</returns>
        public string ReplaceWith {
            get
            {
                string comment = lines[0].Comment;
                if (string.IsNullOrEmpty(comment))
                {
                    return null;
                }
                if (AlwaysReplace)
                {
                    return Name;
                }

                string[] commentTokens = comment.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < commentTokens.Length; ++i)
                {
                    if (commentTokens[i].ToLower() == "replacewith" && commentTokens.Length > i + 1)
                    {
                        return commentTokens[i + 1];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Finds unconditional replacement annotation for a block with the same name.
        /// </summary>
        /// <returns>Whether this block is always annotated to be replaced.</returns>
        public bool AlwaysReplace
        {
            get
            {
                string comment = lines[0].Comment;
                if (string.IsNullOrEmpty(comment))
                {
                    return false;
                }

                string[] commentTokens = comment.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commentToken in commentTokens)
                {
                    if (commentToken.ToLower() == "alwaysreplace")
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public Block(string name, FileType type = FileType.Misc, Source source = Source.Unknown)
        {
            lines = new List<Line>();
            Name = name;
            Type = type;
            Source = source;
        }

        public Block(string name, Line line, FileType type = FileType.Misc, Source source = Source.Unknown) : this(name, type, source)
        {
            Name = name;
            lines.Add(line);
        }

        public Block(string name, List<Line> lines, FileType type = FileType.Misc, Source source = Source.Unknown)
        {
            Name = name;
            this.lines = lines;
            Type = type;
            Source = source;
        }

        public void Add(Line line)
        {
            lines.Add(line);
        }

        public List<FormattedText> GetFormattedTexts()
        {
            List<FormattedText> formattedTexts = new List<FormattedText>();

            FormattedText text = new FormattedText();
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];
                Line nextLine = null;
                if (i < lines.Count - 1)
                {
                    nextLine = lines[i + 1];
                }

                if ((line.Text.StartsWith("INCLUDE") || line.Text.StartsWith("INCBIN")) && line.Text.Contains("/"))
                {
                    continue;
                }
                if (line.Text.StartsWith("assert") || line.Text.StartsWith("fail"))
                {
                    continue;
                }

                switch (Type)
                {
                    case FileType.TextBox:
                    {
                        if (line.Text.StartsWith("text "))
                        {
                            if (line.formattedText != null)
                            {
                                text.Add(line.formattedText.Text);
                            }
                        }
                        else if (line.Text.StartsWith("line ") || line.Text.StartsWith("para ") || line.Text.StartsWith("cont ") || line.Text.StartsWith("text_low") || line.Text.StartsWith("next "))
                        {
                            if (!string.IsNullOrWhiteSpace(text?.Text))
                            {
                                formattedTexts.Add(text);
                                text = new FormattedText();
                            }
                            if (line.formattedText != null)
                            {
                                text.Add(line.formattedText.Text);
                            }
                        }
                        else if (line.Text.StartsWith("text_decimal"))
                        {
                            string[] split = line.Text.Split(' ');
                            int length;
                            if (int.TryParse(split[split.Length - 1], out length))
                            {
                                for (int j = 0; j < length; ++j)
                                {
                                    text.Add(((char)('0' + j)).ToString());
                                }
                            }
                        }
                        else if (line.Text.StartsWith("text_ram"))
                        {
                            string[] split = line.Text.Split(' ');
                            if (split.Length > 1)
                            {
                                if (FormattedText.ramLengths.ContainsKey(split[1]))
                                {
                                    for (int j = 0; j < FormattedText.ramLengths[split[1]]; ++j)
                                    {
                                        text.Add(split[1][j % split[1].Length].ToString());
                                    }
                                }
                                else if (line.Comment.Contains("MaxLength "))
                                {
                                    string[] commentWords = line.Comment.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    int length = -1;
                                    for (int j = 0; j < commentWords.Length; ++j)
                                    {
                                        if (commentWords[j] == "MaxLength" && j <= commentWords.Length - 2)
                                        {
                                            if (!int.TryParse(commentWords[j + 1], out length))
                                            {
                                                length = -1;
                                                FieldInfo[] fields = typeof(Validator).GetFields(BindingFlags.Public | BindingFlags.Static);
                                                foreach (FieldInfo field in fields)
                                                {
                                                    if (field.Name == commentWords[j + 1])
                                                    {
                                                        length = (int)field.GetValue(null);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (length >= 0)
                                    {
                                        for (int j = 0; j < length; ++j)
                                        {
                                            text.Add(split[1][j % split[1].Length].ToString());
                                        }
                                    }
                                    else
                                    {
                                        text.Add(split[1], true);
                                    }
                                }
                                else
                                {
                                    text.Add(split[1], true);
                                }
                            }
                            else
                            {
                                text.Add(split[1], true);
                            }
                        }
                        else if (line.Text == "prompt" || line.Text == "done" || line.Text == "text_end")
                        {
                            if (!string.IsNullOrWhiteSpace(text?.Text))
                            {
                                formattedTexts.Add(text);
                                text = new FormattedText();
                            }
                        }
                        break;
                    }
                    case FileType.Pokedex:
                    {
                        if (line.Text.StartsWith("db ") || line.Text.StartsWith("next ") || line.Text.StartsWith("page "))
                        {
                            if (!string.IsNullOrWhiteSpace(text?.Text))
                            {
                                formattedTexts.Add(text);
                                text = new FormattedText();
                            }
                            if (line.formattedText != null)
                            {
                                text.Add(line.formattedText.Text);
                            }
                        }
                        break;
                    }
                    case FileType.Move:
                    case FileType.Item:
                    case FileType.TrainerClass:
                    {
                        if (line.Text.StartsWith("li "))
                        {
                            if (!string.IsNullOrWhiteSpace(text?.Text))
                            {
                                formattedTexts.Add(text);
                                text = new FormattedText();
                            }
                            if (line.formattedText != null)
                            {
                                text.Add(line.formattedText.Text);
                            }
                        }
                        break;
                    }
                    case FileType.Type:
                    case FileType.Landmark:
                    case FileType.Pokemon:
                    {
                        if (line.Text.Contains("db \""))
                        {
                            if (!string.IsNullOrWhiteSpace(text?.Text))
                            {
                                formattedTexts.Add(text);
                                text = new FormattedText();
                            }
                            if (line.formattedText != null)
                            {
                                text.Add(line.formattedText.Text);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        // Just add every found string
                        if (line.formattedText != null)
                        {
                            text.Add(line.formattedText.Text);
                            formattedTexts.Add(text);
                            text = new FormattedText();
                        }
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(text?.Text))
            {
                formattedTexts.Add(text);
            }

            return formattedTexts;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name).Append(" {\n");
            foreach (Line line in lines)
            {
                sb.Append('\t').Append(line.Text).Append("\n");
            }
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Gets a List of strings in the <see cref="Line"/>s in this <see cref="Block"/>, with whitespace trimmed,
        /// with no comments and excluding empty strings.
        /// </summary>
        /// <returns>The list of strings.</returns>
        public List<string> GetLinesSanitized()
        {
            List<string> strings = new List<string>(lines.Count);
            foreach (Line line in lines)
            {
                string sanitized = line.Text.Trim();
                if (sanitized.Length > 0)
                {
                    strings.Add(sanitized);
                }
            }
            return strings;
        }
    }
    
    // Line.cs
    internal class Line
    {
        public int Number { get; private set; }
        private string _raw;
        public string Raw
        {
            get
            {
                return _raw;
            }
            set
            {
                _raw = value;
                StringBuilder sb = new StringBuilder();
                bool inQuotes = false;
                string comment = "";

                for (int j = 0; j < value.Length; ++j)
                {
                    if (value[j] == '\"')
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (!inQuotes)
                    {
                        if (value[j] == ';')
                        {
                            comment = value.Substring(j + 1).Trim();
                            break;
                        }
                    }

                    sb.Append(value[j]);
                }

                Text = sb.ToString().Trim();
                Comment = comment;

                string stringLiteral = GetFormattedText();
                if (stringLiteral != null)
                {
                    formattedText = new FormattedText(stringLiteral);
                }
            }
        }
        public string Text { get; private set; }
        public string Comment { get; private set; }
        public FormattedText formattedText;

        public Line(int number, string text)
        {
            Number = number;
            Raw = text;
        }

        public Line(string text, string comment)
        {
            Raw = text + " ; " + comment;
        }

        public string GetFormattedText()
        {
            if (Text.Contains('\"'))
            {
                StringBuilder sb = new StringBuilder();
                bool inQuotes = false;
                for (int j = 0; j < Text.Length; ++j)
                {
                    if (Text[j] == '\"')
                    {
                        inQuotes = !inQuotes;
                        continue;
                    }

                    if (inQuotes)
                    {
                        sb.Append(Text[j]);
                    }
                    else
                    {
                        if (Text[j] == ';')
                        {
                            break;
                        }
                    }
                }

                return sb.ToString();
            }

            return null;
        }

        public override string ToString()
        {
            return Text;
        }
    }
    
    // FormattedText.cs
    /// <summary> Class to represent one line of ingame text, even if strings are concatenated on multiple lines of code (with text_ram for example). </summary>
    internal class FormattedText
    {
        public static readonly Dictionary<string, int> maxLengths = new Dictionary<string, int>
        {
            { "<NULL>", 0 },
            { "<PLAY_G>", 7 },
            { "<MOBILE>", 1 },
            { "<CR>", 1 },
            { "<BSP>", 1 },
            { "<LF>", 1 },
            { "<POKE>", 2 },
            { "<WBR>", 0 },
            { "<RED>", 3 },
            { "<GREEN>", 5 },
            { "<ENEMY>", 17 }, // Composed of "Trainer Class + Name"
            { "<MOM>", 3 },
            { "<PKMN>", 2 },
            { "<_CONT>", 0 },
            { "<SCROLL>", 0 },
            { "<NEXT>", 0 },
            { "<LINE>", 0 },
            { "@", 0 },
            { "<PARA>", 0 },
            { "<PLAYER>", 7 },
            { "<RIVAL>", 7 },
            { "#", 4 },
            { "<CONT>", 0 },
            { "<……>", 2 },
            { "<DONE>", 0 },
            { "<PROMPT>", 0 },
            { "<TARGET>", 17 }, // Can be "Enemy <Pokemon>"
            { "<USER>", 17 }, // Can be "Enemy <Pokemon>"
            { "<PC>", 2 },
            { "<TM>", 2 },
            { "<TRAINER>" , 7 },
            { "<ROCKET>", 6 },
            { "<DEXEND>", 0 },
            { "<BOLD_A>", 1 },
            { "<BOLD_B>", 1 },
            { "<BOLD_C>", 1 },
            { "<BOLD_D>", 1 },
            { "<BOLD_E>", 1 },
            { "<BOLD_F>", 1 },
            { "<BOLD_G>", 1 },
            { "<BOLD_H>", 1 },
            { "<BOLD_I>", 1 },
            { "<BOLD_V>", 1 },
            { "<BOLD_S>", 1 },
            { "<BOLD_L>", 1 },
            { "<BOLD_M>", 1 },
            { "<COLON>", 1 },
            { "<PO>", 1 },
            { "<KE>", 1 },
            { "<LV>", 1 },
            { "<DO>", 1 },
            { "<ID>", 1 },
            { "'d", 1 },
            { "'l", 1 },
            { "'m", 1 },
            { "'r", 1 },
            { "'s", 1 },
            { "'t", 1 },
            { "'v", 1 },
            { "{d:NUM_TMS}", 2 },
            { "{d:BLUE_CARD_POINT_CAP}", 2 },
            { "{d:BUG_CONTEST_BALLS}", 2 },
            { "{d:BUG_CONTEST_MINUTES}", 2 },
            { "{d:ROUTE39FARMHOUSE_MILK_PRICE}", 3 },
            { "{d:ROUTE43GATE_TOLL}", 4 },
            { "{d:NUM_UNOWN}", 2 },
        };
        public static readonly Dictionary<string, int> ramLengths = new Dictionary<string, int>
        {
            { "wSeerCaughtLevelString", 3 },

            { "wPlayerTrademonSenderName", Validator.MaxPlayerNameLength },
            { "wOTTrademonSenderName", Validator.MaxPlayerNameLength },
            { "wPlayerName", Validator.MaxPlayerNameLength },
            { "wMysteryGiftPartnerName", Validator.MaxPlayerNameLength },
            { "wMysteryGiftPlayerName", Validator.MaxPlayerNameLength },
            { "wMysteryGiftCardHolderName", Validator.MaxPlayerNameLength },
            { "wMagikarpRecordHoldersName", Validator.MaxPlayerNameLength },
            { "wSeerOT", Validator.MaxPlayerNameLength },
            { "wMobileParticipant1Nickname", Validator.MaxPlayerNameLength },
            { "wMobileParticipant2Nickname", Validator.MaxPlayerNameLength },
            { "wMobileParticipant3Nickname", Validator.MaxPlayerNameLength },
            { "wSeerTimeOfDay", 7 },

            { "wBugContestWinnerName", Validator.MaxTrainerNameLength },

            { "wEnemyMonNickname", Validator.MaxPokemonNameLength },
            { "wBattleMonNickname", Validator.MaxPokemonNameLength },
            { "wPlayerTrademonSpeciesName", Validator.MaxPokemonNameLength },
            { "wOTTrademonSpeciesName", Validator.MaxPokemonNameLength },
            { "wBreedMon2Nickname", Validator.MaxPokemonNameLength },
            { "wBreedMon1Nickname", Validator.MaxPokemonNameLength },
            { "wBufferTrademonNickname", Validator.MaxPokemonNameLength },
            { "wSeerNickname", Validator.MaxPokemonNameLength },

            { "wSeerCaughtLocation", Validator.MaxLandmarkLength },
        };

        public static char Terminator = '@';

        /// <summary>  Raw text. </summary>
        public string Text { get; private set; }
        /// <summary> The remaining text after the formatted strings are stripped out. </summary>
        public string Unformatted { get; private set; }
        /// <summary> Length of text, including maximum lengths of formatted parts. </summary>
        public int Length { get; private set; }
        /// <summary> Can length of text be reliably determined? </summary>
        public bool LengthUnknown { get; private set; }
        /// <summary> Does the text have an '@' terminator? </summary>
        public bool IsTerminated { get; private set; }

        public FormattedText()
        {
            Text = string.Empty;
            Unformatted = string.Empty;
        }

        public FormattedText(string text, bool lengthUnknown = false)
        {
            Add(text);
            LengthUnknown = lengthUnknown;
        }

        /// <summary>
        /// Adds <paramref name="text"/> to this formatted text.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <param name="lengthUnknown"><c>true</c> if something about this text makes it so length cannot be determined (text_ram with ambiguous length).</param>
        public void Add(string text, bool lengthUnknown = false)
        {
            Text += text;

            foreach (KeyValuePair<string, int> pair in maxLengths)
            {
                int lengthBefore = text.Length;
                text = text.Replace(pair.Key, string.Empty);
                Length += ((lengthBefore - text.Length) / pair.Key.Length) * pair.Value;
            }

            Length += text.Length;
            Unformatted = text;
            LengthUnknown |= lengthUnknown;
            IsTerminated = Text.EndsWith("@");
        }

        public override string ToString()
        {
            return Text;
        }
    }
    
    // Validator.cs
    internal class Validator
    {
        public const int MaxTextboxLength = 18;
        public const int MaxPlayerNameLength = 7;
        public const int MaxTrainerNameLength = 10; // "ANN & ANNE"
        public const int MaxTrainerClassNameLength = 13; // "POKéMON PROF."
        public const int MaxPokemonNameLength = 10;
        public const int MaxMoveNameLength = 12;
        public const int MaxItemNameLength = 12;
        public const int MaxTypeNameLength = 8;
        public const int MaxStatNameLength = 8;
        public const int MaxBagPocketNameLength = 11;
        public const int MaxDecorationNameLength = 17;

        public const int MaxPokedexLength = 18;
        public const int MaxSpeciesNameLength = 11;

        public const int MaxLandmarkLineLength = 11;
        public const int MaxLandmarkLength = 17;

        public const string PrintableChars = "“”·… ′″ABCDEFGHIJKLMNOPQRSTUVWXYZ():;[]abcdefghijklmnopqrstuvwxyzàèùßçÄÖÜäöüëïâôûêîÏË←ÈÉ'-+?!.&é→▷▶▼♂¥×/,♀0123456789┌─┐│└─┘";

        private Block _block;
        private StringBuilder output;

        public Validator(Block block)
        {
            _block = block;
            output = new StringBuilder();
        }

        public Validator(List<Line> lines)
        {
            _block = new Block(lines[0].Text, lines);
            output = new StringBuilder();
        }

        public string Validate()
        {
            ValidateLines();
            ValidateText();
            return output.Length > 0 ? output.ToString() : null;
        }

        private void ValidateLines()
        {
            List<string> lines = _block.GetLinesSanitized();
            if (_block.Type == FileType.TextBox)
            {
                for (int i = 0; i < lines.Count; ++i)
                {
                    string line = lines[i];
                    if (lines.Count > i + 1)
                    {
                        string nextLine = lines[i + 1];
                        if (CheckTextboxLineNeedsTerminator(line, nextLine) && !lines[i].EndsWith("@\""))
                        {
                            output.Append(GetTerminatorErrorMessage(line));
                        }
                        if (!CheckTextboxNextLineValid(line, nextLine))
                        {
                            output.Append(GetInvalidNextLineErrorMessage(line, nextLine));
                        }
                    }
                    else
                    {
                        if (!CheckTextboxNextLineValid(line, null))
                        {
                            output.Append(GetInvalidLastLineErrorMessage(line));
                        }
                    }
                }
            }
        }

        private void ValidateText()
        {
            int maxLength = -1;
            bool mustTerminate = false;
            switch (_block.Type)
            {
                case FileType.TextBox:
                {
                    maxLength = MaxTextboxLength;
                    break;
                }
                case FileType.Pokedex:
                {
                    maxLength = MaxPokedexLength;
                    break;
                }
                case FileType.Item:
                {
                    maxLength = MaxItemNameLength;
                    break;
                }
                case FileType.Pokemon:
                {
                    maxLength = MaxPokemonNameLength;
                    break;
                }
                case FileType.Move:
                {
                    maxLength = MaxMoveNameLength;
                    break;
                }
                case FileType.Type:
                {
                    mustTerminate = true;
                    maxLength = MaxTypeNameLength;
                    break;
                }
                case FileType.Landmark:
                {
                    mustTerminate = true;
                    maxLength = MaxLandmarkLength;
                    break;
                }
                case FileType.TrainerClass:
                {
                    maxLength = MaxTrainerClassNameLength;
                    break;
                }
            }

            List<FormattedText> text = _block.GetFormattedTexts();
            for (int i = 0; i < text.Count; ++i)
            {
                FormattedText formattedText = text[i];
                FormattedText nextText = null;
                if (text.Count > i + 1)
                {
                    nextText = text[i + 1];
                }

                if (string.IsNullOrWhiteSpace(formattedText?.Text))
                {
                    continue;
                }
                if ((formattedText.Text.Contains("INCLUDE") || formattedText.Text.Contains("INCBIN")) && formattedText.Text.Contains("/"))
                {
                    continue;
                }
                if (formattedText.Text.StartsWith("assert") || formattedText.Text.StartsWith("fail"))
                {
                    continue;
                }

                StringBuilder unmappedCharactersBuilder = new StringBuilder();
                foreach (char c in formattedText.Unformatted)
                {
                    if (!PrintableChars.Contains(c.ToString()))
                    {
                        unmappedCharactersBuilder.Append(c);
                    }
                }
                if (unmappedCharactersBuilder.ToString() != string.Empty)
                {
                    output.Append(GetUnmappedCharErrorMessage(formattedText, unmappedCharactersBuilder.ToString()));
                }

                if (_block.Type == FileType.Pokedex)
                {
                    if (i == 0)
                    {
                        // First string is species name
                        string message = GetLengthValidationMessage(formattedText, MaxSpeciesNameLength);
                        if (!string.IsNullOrEmpty(message))
                        {
                            output.Append(message);
                        }
                        if (!formattedText.Text.EndsWith(FormattedText.Terminator.ToString()))
                        {
                            output.Append(GetTerminatorErrorMessage(formattedText.Text));
                        }
                    }
                    else
                    {
                        if (i == text.Count - 1)
                        {
                            // last string is last line of entry
                            if (!formattedText.Text.EndsWith(FormattedText.Terminator.ToString()))
                            {
                                output.Append(GetTerminatorErrorMessage(formattedText.Text));
                            }
                        }
                        string message = GetLengthValidationMessage(formattedText, maxLength);
                        if (!string.IsNullOrEmpty(message))
                        {
                            output.Append(message);
                        }
                    }
                }
                else if (_block.Type == FileType.Pokemon)
                {
                    if (formattedText.Text.Length != MaxPokemonNameLength)
                    {
                        output.Append(GetLengthExactlyErrorMessage(formattedText, MaxPokemonNameLength));
                    }
                }
                else if (_block.Type != FileType.Misc)
                {
                    string message = GetLengthValidationMessage(formattedText, maxLength);
                    if (!string.IsNullOrEmpty(message))
                    {
                        output.Append(message);
                    }
                }

                if (mustTerminate)
                {
                    if (!formattedText.Text.EndsWith(FormattedText.Terminator.ToString()))
                    {
                        output.Append(GetTerminatorErrorMessage(formattedText.Text));
                    }
                }

                if (_block.Type == FileType.Landmark)
                {
                    string message = GetLandmarkLineValidationMessage(formattedText, MaxLandmarkLineLength);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        output.Append(message);
                    }
                }

                if (_block.Source == Source.GoldSilver && formattedText.Text.Contains("<PLAY_G>"))
                {
                    output.Append(GetGoldSilverPLAYGUnsupportedMessage(formattedText));
                }
            }
        }

        public bool CheckTextboxNextLineValid(string line, string nextLine)
        {
            if (nextLine == null)
            {
                if (line == "text_end")
                {
                    return true;
                }

                if (line.StartsWith("line ") ||
                    line.StartsWith("para ") ||
                    line.StartsWith("cont ") ||
                    line.StartsWith("text_") ||
                    line.StartsWith("sound_"))
                {
                    return false;
                }

                return true;
            }

            if (nextLine.StartsWith("if DEF") ||
                nextLine.StartsWith("else") ||
                nextLine.StartsWith("endc"))
            {
                // These are rare enough that they can be checked manually
                return true;
            }

            if (line.StartsWith("text "))
            {
                if (nextLine.StartsWith("line ") ||
                    nextLine.StartsWith("para ") ||
                    nextLine.StartsWith("cont ") ||
                    nextLine.StartsWith("next ") ||
                    nextLine == "prompt" ||
                    nextLine == "done" ||
                    nextLine.StartsWith("text_") ||
                    nextLine.StartsWith("sound_"))
                {
                    return true;
                }
            }
            else if (line.StartsWith("line ") || line.StartsWith("cont "))
            {
                if (nextLine.StartsWith("para ") ||
                    nextLine.StartsWith("cont ") ||
                    nextLine == "prompt" ||
                    nextLine == "done" ||
                    nextLine.StartsWith("text_") ||
                    nextLine.StartsWith("sound_"))
                {
                    return true;
                }
            }
            else if (line.StartsWith("text_ram ") || line.StartsWith("text_decimal ") || line == "text_promptbutton")
            {
                if (nextLine.StartsWith("text ") ||
                    nextLine.StartsWith("text_"))
                {
                    return true;
                }
            }
            else if (line == "text_end" || line == "prompt" || line == "done")
            {
                return true;
            }
            else
            {
                return true;
            }

            return false;
        }

        private static bool CheckTextboxLineNeedsTerminator(string line, string nextLine)
        {

            if (line.StartsWith("text ")
                || line.StartsWith("line ")
                || line.StartsWith("para ")
                || line.StartsWith("cont ")
                || line.StartsWith("next "))
            {
                if (nextLine.StartsWith("sound_")
                    || nextLine.StartsWith("text_"))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetLengthValidationMessage(FormattedText formattedText, int maxLength)
        {
            StringBuilder sb = new StringBuilder();

            // Length validation
            if (formattedText.LengthUnknown)
            {
                sb.Append(GetCannotGetLengthMessage(formattedText));
            }
            else if (maxLength > 0 && formattedText.Length > maxLength)
            {
                sb.Append(GetLengthErrorMessage(formattedText, maxLength));
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        private string GetCannotGetLengthMessage(FormattedText formattedText)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tWarning: Cannot determine length of \"").Append(formattedText.Text).Append("\" in ").Append(_block.Name).Append(" automatically. Please add MaxLength annotation.\n").ToString();
        }

        private string GetLengthErrorMessage(FormattedText formattedText, int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(formattedText.Text).Append("\" in ").Append(_block.Name).Append(" may be ").Append(formattedText.Length).Append(" chars! Text can be at most ").Append(maxLength).Append(" characters long.\n").ToString();
        }

        private string GetLandmarkLineValidationMessage(FormattedText formattedText, int maxLength)
        {
            int indexOfFirstBSP = formattedText.Text.IndexOf("<BSP>");

            if (indexOfFirstBSP > maxLength || (formattedText.Length - indexOfFirstBSP - 1) > maxLength)
            {
                StringBuilder sb = new StringBuilder();
                return sb.Append("\tError: text \"").Append(formattedText.Text).Append("\" has a line too long for a landmark name. Lines can be at most ").Append(maxLength).Append(" characters long. Consider using a <BSP>.\n").ToString();
            }
            return null;
        }

        private string GetTerminatorErrorMessage(string text)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(text).Append("\" in ").Append(_block.Name).Append(" is missing a terminator! Add @ to the end of the string.\n").ToString();
        }
        private string GetInvalidNextLineErrorMessage(string text, string nextLine)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(text).Append("\" in ").Append(_block.Name).Append(" cannot be followed by \"").Append(nextLine).Append("\".\n").ToString();
        }
        private string GetInvalidLastLineErrorMessage(string text)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: Text block \"").Append(_block.Name).Append(" cannot end with \"").Append(text).Append("\".\n").ToString();
        }

        private string GetLengthExactlyErrorMessage(FormattedText formattedText, int length)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(formattedText.Text).Append("\" in ").Append(_block.Name).Append(" must be exactly ").Append(length).Append(" chars long.\n").ToString();
        }

        private string GetUnmappedCharErrorMessage(FormattedText formattedText, string unmapped)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(formattedText.Text).Append("\" in ").Append(_block.Name).Append(" contains unmapped characters \'").Append(unmapped).Append("\'.\n").ToString();
        }

        private string GetGoldSilverPLAYGUnsupportedMessage(FormattedText formattedText)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append("\tError: text \"").Append(formattedText.Text).Append("\" in ").Append(_block.Name).Append(" contains \"<PLAY_G>\" which is unsupported in Gold/Silver. Use \"<PLAYER>\" instead.\n").ToString();
        }
    }
    
    // Comparer.cs
    internal class Comparer
    {
        private readonly ASMFile source;
        private readonly ASMFile target;

        public readonly List<Block> added;
        public readonly List<Block> removed;
        public readonly Dictionary<Block, Block> modified;
        public readonly Dictionary<Block, Block> matched;

        private bool canBeMergedFully = true;

        public Comparer(ASMFile source, ASMFile target)
        {
            this.source = source;
            this.target = target;

            added = new List<Block>();
            removed = new List<Block>();
            modified = new Dictionary<Block, Block>();
            matched = new Dictionary<Block, Block>();
        }

        public string Compare()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Comparing ").Append(source.File.FullName).Append(" to ").Append(target.File.FullName).Append('\n');
            List<string> targetBlockNames = target.blocks.Keys.ToList();

            // Cannot modify list while iterating over it.
            foreach (string name in targetBlockNames.ToArray())
            {
                if (target.blocks[name].lines.Count < 1)
                {
                    continue;
                }
            }

            foreach (KeyValuePair<string, Block> pair in source.blocks)
            {
                if (matched.ContainsKey(pair.Value) || modified.ContainsKey(pair.Value))
                {
                    targetBlockNames.Remove(pair.Key);
                    continue;
                }

                if (!target.blocks.ContainsKey(pair.Key))
                {
                    removed.Add(pair.Value);
                    sb.Append("\t- Block ").Append(pair.Key).Append(" deleted in target.\n");
                }
                else
                {
                    targetBlockNames.Remove(pair.Key);
                    sb.Append(CompareBlocks(pair.Value, target.blocks[pair.Key]));
                }
            }

            foreach (string name in targetBlockNames)
            {
                added.Add(target.blocks[name]);
                canBeMergedFully = false;
                sb.Append("\t+ Block ").Append(name).Append(" added in target.\n");
            }

            if (canBeMergedFully)
            {
                sb.Append("\tSource ").Append(source.Name).Append(" can be merged fully into target ").Append(target.Name).Append('\n');
            }

            return sb.ToString();
        }

        private string CompareBlocks(Block sourceBlock, Block targetBlock)
        {
            StringBuilder sb = new StringBuilder();

            List<string> sourceLines = sourceBlock.GetLinesSanitized();
            List<string> targetLines = targetBlock.GetLinesSanitized();
            if (sourceLines.Count != targetLines.Count)
            {
                modified.Add(sourceBlock, targetBlock);
                canBeMergedFully = false;
                sb.Append("\t! Block ").Append(sourceBlock.Name).Append(" has different line counts between source and target.\n");
            }
            else
            {
                bool changed = false;
                for (int i = 0; i < sourceLines.Count; i++)
                {
                    if (i == 0 && (sourceBlock.Name != "default" || sourceBlock.Name != targetBlock.Name))
                    {
                        continue;
                    }

                    string sourceLine = sourceLines[i];
                    string targetLine = targetLines[i];
                    if (source.Source == Source.GoldSilver || target.Source == Source.GoldSilver)
                    {
                        sourceLine = sourceLine.Replace("<PLAY_G>", "<PLAYER>");
                        targetLine = targetLine.Replace("<PLAY_G>", "<PLAYER>");
                    }
                    if (sourceLine != targetLine)
                    {
                        if (!changed)
                        {
                            sb.Append("\t! Block ").Append(sourceBlock.Name).Append(" in Source content differs from ").Append(targetBlock.Name).Append(" in Target.\n");
                            changed = true;
                        }
                        sb.Append("\t\t")
                          .Append(i)
                          .Append(":\n\t\t\t- Source: ")
                          .Append(sourceLine)
                          .Append("\n\t\t\t+ Target: ")
                          .Append(targetLine)
                          .Append('\n');
                    }
                }

                if (changed)
                {
                    modified.Add(sourceBlock, targetBlock);
                    canBeMergedFully = false;
                }
                else
                {
                    matched.Add(sourceBlock, targetBlock);
                    sb.Append("\t= Block ").Append(sourceBlock.Name).Append(" in Source matches ").Append(targetBlock.Name).Append(" in Target.\n");
                }
            }
            return sb.ToString();
        }
    }
    
    // Merger.cs
    internal class Merger
    {

        private readonly ASMFile source;
        private readonly ASMFile target;
        private readonly ASMFile baseSource;
        private readonly ASMFile baseTarget;
        private Comparer baseComparison;

        private StringBuilder output;

        public Merger(ASMFile source, ASMFile target, ASMFile baseSource, ASMFile baseTarget)
        {
            this.source = source;
            this.target = target;
            this.baseSource = baseSource;
            this.baseTarget = baseTarget;

            output = new StringBuilder();
        }

        public string Merge()
        {
            output.Append("Performing comparison between bases...\n");
            baseComparison = new Comparer(baseSource, baseTarget);
            output.AppendLine(baseComparison.Compare());

            HandleAdded();
            HandleMatched();
            HandleModified();

            return output.ToString();
        }

        /// <summary>
        /// Checks for and handles unconditional replacement annotations in blocks added in target, and if found,
        /// raplaces text in target blocks with specified source blocks regardless of base comparisons. Otherwise does nothing.
        /// </summary>
        private void HandleAdded()
        {
            foreach (Block added in baseComparison.added)
            {
                if (target.blocks.ContainsKey(added.Name))
                {
                    Block targetBlock = target.blocks[added.Name];
                    string replacement = targetBlock.ReplaceWith;
                    if (replacement != null && source.blocks.ContainsKey(replacement))
                    {
                        Block sourceBlock = source.blocks[replacement];
                        if (sourceBlock.GetFormattedTexts().Count == 0)
                        {
                            continue;
                        }

                        CopyLines(sourceBlock, targetBlock);
                        output.Append("Replacing block ").Append(targetBlock.Name).Append(" in target with ").Append(sourceBlock.Name).Append(" from source unconditionally.\n");
                    }
                }
            }
        }

        /// <summary>
        /// Replaces text in target blocks with text from source blocks, if the base comparisons matched.
        /// </summary>
        private void HandleMatched()
        {
            foreach (KeyValuePair<Block, Block> match in baseComparison.matched)
            {
                if (target.blocks.ContainsKey(match.Key.Name) && source.blocks.ContainsKey(match.Key.Name))
                {
                    Block sourceBlock = source.blocks[match.Key.Name];
                    if (sourceBlock.GetFormattedTexts().Count == 0)
                    {
                        continue;
                    }
                    Block targetBlock = target.blocks[match.Value.Name];

                    CopyLines(sourceBlock, targetBlock);
                    output.Append("Replacing block ").Append(targetBlock.Name).Append(" in target with ").Append(sourceBlock.Name).Append(" from source.\n");
                }
            }
        }

        /// <summary>
        /// Checks for and handles AlwaysReplace annotations in blocks in target, and if found, replaces
        /// text in target blocks with source blocks regardless of base comparisons. Otherwise does nothing.
        /// </summary>
        private void HandleModified()
        {
            foreach (KeyValuePair<Block, Block> modifiedBlock in baseComparison.modified)
            {
                if (modifiedBlock.Value.GetFormattedTexts().Count == 0)
                {
                    continue;
                }

                if (target.blocks.ContainsKey(modifiedBlock.Key.Name) && source.blocks.ContainsKey(modifiedBlock.Key.Name))
                {
                    Block targetBlock = target.blocks[modifiedBlock.Key.Name];
                    if (targetBlock.AlwaysReplace && source.blocks.ContainsKey(targetBlock.Name))
                    {
                        Block sourceBlock = source.blocks[targetBlock.Name];
                        if (sourceBlock.GetFormattedTexts().Count == 0)
                        {
                            continue;
                        }

                        CopyLines(sourceBlock, targetBlock);
                        output.Append("Replacing block ").Append(targetBlock.Name).Append(" in target with ").Append(sourceBlock.Name).Append(" from source unconditionally.\n");
                    }
                    else
                    {
                        output.Append("Block ").Append(modifiedBlock.Key.Name).Append(" differs in base scripts. Cannot merge automatically!\n");
                    }
                }
            }
        }

        /// <summary>
        /// Copies all lines from <paramref name="sourceBlock"/> into <paramref name="targetBlock"/>.
        /// </summary>
        /// <param name="sourceBlock">The source <see cref="Block"/>.</param>
        /// <param name="targetBlock">The target <see cref="Block"/>.</param>
        private void CopyLines(Block sourceBlock, Block targetBlock)
        {
            List<Line> newLines = new List<Line>();
            for (int i = 0; i < sourceBlock.lines.Count; ++i)
            {
                if (i == 0 && sourceBlock.Name != "default")
                {
                    // Keep the first line from target, so block names are the same in target.
                    newLines.Add(targetBlock.lines[i]);
                }
                else
                {
                    Line line = sourceBlock.lines[i];
                    string raw = line.Raw;
                    if (source.Source == Source.GoldSilver || target.Source == Source.GoldSilver)
                    {
                        raw = raw.Replace("<PLAY_G>", "<PLAYER>");
                    }
                    newLines.Add(new Line(line.Number, raw));
                }
            }

            targetBlock.lines = newLines;
        }

    }
}