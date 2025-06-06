	object_const_def
	const ROUTE36NATIONALPARKGATE_OFFICER1
	const ROUTE36NATIONALPARKGATE_YOUNGSTER1
	const ROUTE36NATIONALPARKGATE_YOUNGSTER2
	const ROUTE36NATIONALPARKGATE_ROCKER
	const ROUTE36NATIONALPARKGATE_POKEFAN_M
	const ROUTE36NATIONALPARKGATE_YOUNGSTER3
	const ROUTE36NATIONALPARKGATE_YOUNGSTER4
	const ROUTE36NATIONALPARKGATE_LASS
	const ROUTE36NATIONALPARKGATE_YOUNGSTER5
	const ROUTE36NATIONALPARKGATE_YOUNGSTER6
	const ROUTE36NATIONALPARKGATE_YOUNGSTER7
	const ROUTE36NATIONALPARKGATE_OFFICER2

Route36NationalParkGate_MapScripts:
	def_scene_scripts
	scene_script Route36NationalParkGateNoop1Scene,             SCENE_ROUTE36NATIONALPARKGATE_NOOP
	scene_script Route36NationalParkGateNoop2Scene,             SCENE_ROUTE36NATIONALPARKGATE_UNUSED
	scene_script Route36NationalParkGateLeaveContestEarlyScene, SCENE_ROUTE36NATIONALPARKGATE_LEAVE_CONTEST_EARLY

	def_callbacks
	callback MAPCALLBACK_NEWMAP, Route36NationalParkGateCheckIfContestRunningCallback
	callback MAPCALLBACK_OBJECTS, Route36NationalParkGateCheckIfContestAvailableCallback

Route36NationalParkGateNoop1Scene:
	end

Route36NationalParkGateNoop2Scene:
	end

Route36NationalParkGateLeaveContestEarlyScene:
	sdefer Route36NationalParkGateLeavingContestEarlyScript
	end

Route36NationalParkGateCheckIfContestRunningCallback:
	checkflag ENGINE_BUG_CONTEST_TIMER
	iftrue .BugContestIsRunning
	setscene SCENE_ROUTE36NATIONALPARKGATE_NOOP
	endcallback

.BugContestIsRunning:
	setscene SCENE_ROUTE36NATIONALPARKGATE_LEAVE_CONTEST_EARLY
	endcallback

Route36NationalParkGateCheckIfContestAvailableCallback:
	checkevent EVENT_WARPED_FROM_ROUTE_35_NATIONAL_PARK_GATE
	iftrue .Return
	readvar VAR_WEEKDAY
	ifequal TUESDAY, .SetContestOfficer
	ifequal THURSDAY, .SetContestOfficer
	ifequal SATURDAY, .SetContestOfficer
	checkflag ENGINE_BUG_CONTEST_TIMER
	iftrue .SetContestOfficer
	disappear ROUTE36NATIONALPARKGATE_OFFICER1
	appear ROUTE36NATIONALPARKGATE_OFFICER2
	endcallback

.SetContestOfficer:
	appear ROUTE36NATIONALPARKGATE_OFFICER1
	disappear ROUTE36NATIONALPARKGATE_OFFICER2
.Return:
	endcallback

Route36NationalParkGateLeavingContestEarlyScript:
	turnobject PLAYER, UP
	opentext
	readvar VAR_CONTESTMINUTES
	addval 1
	getnum STRING_BUFFER_3
	writetext Route36NationalParkGateOfficer1WantToFinishText
	yesorno
	iffalse .GoBackToContest
	writetext Route36NationalParkGateOfficer1WaitHereForAnnouncementText
	waitbutton
	closetext
	special FadeOutToBlack
	special ReloadSpritesNoPalettes
	scall .CopyContestants
	disappear ROUTE36NATIONALPARKGATE_OFFICER1
	appear ROUTE36NATIONALPARKGATE_OFFICER2
	applymovement PLAYER, Route36NationalParkGatePlayerWaitWithContestantsMovement
	pause 15
	special FadeInFromBlack
	jumpstd BugContestResultsScript

.GoBackToContest:
	writetext Route36NationalParkGateOfficer1OkGoFinishText
	waitbutton
	closetext
	turnobject PLAYER, LEFT
	playsound SFX_EXIT_BUILDING
	special FadeOutToWhite
	waitsfx
	warpfacing LEFT, NATIONAL_PARK_BUG_CONTEST, 33, 18
	end

.CopyContestants:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_1A
	iftrue .Not1
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER1
.Not1:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_2A
	iftrue .Not2
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER2
.Not2:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_3A
	iftrue .Not3
	appear ROUTE36NATIONALPARKGATE_ROCKER
.Not3:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_4A
	iftrue .Not4
	appear ROUTE36NATIONALPARKGATE_POKEFAN_M
.Not4:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_5A
	iftrue .Not5
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER3
.Not5:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_6A
	iftrue .Not6
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER4
.Not6:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_7A
	iftrue .Not7
	appear ROUTE36NATIONALPARKGATE_LASS
.Not7:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_8A
	iftrue .Not8
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER5
.Not8:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_9A
	iftrue .Not9
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER6
.Not9:
	checkevent EVENT_BUG_CATCHING_CONTESTANT_10A
	iftrue .Not10
	appear ROUTE36NATIONALPARKGATE_YOUNGSTER7
.Not10:
	special UpdateSprites
	end

Route36OfficerScriptContest:
	readvar VAR_WEEKDAY
	ifequal SUNDAY, _ContestNotOn
	ifequal MONDAY, _ContestNotOn
	ifequal WEDNESDAY, _ContestNotOn
	ifequal FRIDAY, _ContestNotOn
	faceplayer
	opentext
	checkflag ENGINE_DAILY_BUG_CONTEST
	iftrue Route36Officer_ContestHasConcluded
	scall Route36ParkGate_DayToText
	writetext Route36NationalParkGateOfficer1AskToParticipateText
	yesorno
	iffalse .DecidedNotToJoinContest
	readvar VAR_PARTYCOUNT
	ifgreater 1, .LeaveMonsWithOfficer
	special ContestDropOffMons
	clearevent EVENT_LEFT_MONS_WITH_CONTEST_OFFICER
.ResumeStartingContest:
	setflag ENGINE_BUG_CONTEST_TIMER
	special PlayMapMusic
	writetext Route36NationalParkGateOfficer1GiveParkBallsText
	promptbutton
	waitsfx
	writetext Route36NationalParkGatePlayerReceivedParkBallsText
	playsound SFX_ITEM
	waitsfx
	writetext Route36NationalParkGateOfficer1ExplainsRulesText
	waitbutton
	closetext
	setflag ENGINE_BUG_CONTEST_TIMER
	special GiveParkBalls
	turnobject PLAYER, LEFT
	playsound SFX_EXIT_BUILDING
	special FadeOutToWhite
	waitsfx
	special SelectRandomBugContestContestants
	warpfacing LEFT, NATIONAL_PARK_BUG_CONTEST, 33, 18
	end

.LeaveMonsWithOfficer:
	readvar VAR_PARTYCOUNT
	ifless PARTY_LENGTH, .ContinueLeavingMons
	readvar VAR_BOXSPACE
	ifequal 0, .BoxFull
.ContinueLeavingMons:
	special CheckFirstMonIsEgg
	ifequal TRUE, .FirstMonIsEgg
	writetext Route36NationalParkGateOfficer1AskToUseFirstMonText
	yesorno
	iffalse .RefusedToLeaveMons
	special ContestDropOffMons
	iftrue .FirstMonIsFainted
	setevent EVENT_LEFT_MONS_WITH_CONTEST_OFFICER
	writetext Route36NationalParkGateOfficer1WellHoldYourMonText
	promptbutton
	writetext Route36NationalParkGatePlayersMonLeftWithHelperText
	playsound SFX_GOT_SAFARI_BALLS
	waitsfx
	promptbutton
	sjump .ResumeStartingContest

.DecidedNotToJoinContest:
	writetext Route36NationalParkGateOfficer1TakePartInFutureText
	waitbutton
	closetext
	end

.RefusedToLeaveMons:
	writetext Route36NationalParkGateOfficer1ChooseMonAndComeBackText
	waitbutton
	closetext
	end

.FirstMonIsFainted:
	writetext Route36NationalParkGateOfficer1FirstMonCantBattleText
	waitbutton
	closetext
	end

.BoxFull:
	writetext Route36NationalParkGateOfficer1MakeRoomText
	waitbutton
	closetext
	end

.FirstMonIsEgg:
	writetext Route36NationalParkGateOfficer1EggAsFirstMonText
	waitbutton
	closetext
	end

Route36Officer_ContestHasConcluded:
	checkevent EVENT_CONTEST_OFFICER_HAS_SUN_STONE
	iftrue .Sunstone
	checkevent EVENT_CONTEST_OFFICER_HAS_EVERSTONE
	iftrue .Everstone
	checkevent EVENT_CONTEST_OFFICER_HAS_GOLD_BERRY
	iftrue .GoldBerry
	checkevent EVENT_CONTEST_OFFICER_HAS_BERRY
	iftrue .Berry
	writetext Route36NationalParkGateOfficer1ContestIsOverText
	waitbutton
	closetext
	end

.Sunstone:
	writetext Route36NationalParkGateOfficer1HeresThePrizeText
	promptbutton
	verbosegiveitem SUN_STONE
	iffalse .BagFull
	clearevent EVENT_CONTEST_OFFICER_HAS_SUN_STONE
	closetext
	end

.Everstone:
	writetext Route36NationalParkGateOfficer1HeresThePrizeText
	promptbutton
	verbosegiveitem EVERSTONE
	iffalse .BagFull
	clearevent EVENT_CONTEST_OFFICER_HAS_EVERSTONE
	closetext
	end

.GoldBerry:
	writetext Route36NationalParkGateOfficer1HeresThePrizeText
	promptbutton
	verbosegiveitem GOLD_BERRY
	iffalse .BagFull
	clearevent EVENT_CONTEST_OFFICER_HAS_GOLD_BERRY
	closetext
	end

.Berry:
	writetext Route36NationalParkGateOfficer1HeresThePrizeText
	promptbutton
	verbosegiveitem BERRY
	iffalse .BagFull
	clearevent EVENT_CONTEST_OFFICER_HAS_BERRY
	closetext
	end

.BagFull:
	writetext Route36NationalParkGateOfficer1WellHoldPrizeText
	waitbutton
	closetext
	end

_ContestNotOn:
	jumptextfaceplayer Route36NationalParkGateOfficer1SomeMonOnlySeenInParkText

Route36NationalParkGateOfficerScript:
	faceplayer
	opentext
	checkflag ENGINE_DAILY_BUG_CONTEST
	iftrue Route36Officer_ContestHasConcluded
	writetext Route36NationalParkGateOfficer1SomeMonOnlySeenInParkText
	waitbutton
	closetext
	end

Route36ParkGate_DayToText:
	jumpstd DayToTextScript
	end

BugCatchingContestant1BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant1BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant1BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant2BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant2BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant2BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant3BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant3BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant3BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant4BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant4BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant4BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant5BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant5BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant5BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant6BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant6BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant6BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant7BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant7BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant7BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant8BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant8BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant8BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant9BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant9BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant9BStillCompetingText
	waitbutton
	closetext
	end

BugCatchingContestant10BScript:
	faceplayer
	opentext
	checkevent EVENT_TEMPORARY_UNTIL_MAP_RELOAD_1
	iffalse .StillCompeting
	writetext BugCatchingContestant10BText
	waitbutton
	closetext
	end

.StillCompeting:
	writetext BugCatchingContestant10BStillCompetingText
	waitbutton
	closetext
	end

UnusedBugCatchingContestExplanationSign: ; unreferenced
; duplicate of BugCatchingContestExplanationSign in Route35NationalParkGate.asm
	jumptext UnusedBugCatchingContestExplanationText

Route36NationalParkGatePlayerWaitWithContestantsMovement:
	big_step DOWN
	big_step RIGHT
	turn_head UP
	step_end

Route36NationalParkGateOfficer1AskToParticipateText:
	text "Het is @" ; "Today's @"
	text_ram wStringBuffer3 ; Day of the week MaxLength 9
	text "."
	line "De dag van het" ; "That means the"

	para "wekelijkse" ; "Bug-Catching Con-"
	line "KEVERTOERNOOI." ; "test is on today."

	para "De regels zijn" ; "The rules are sim-"
	line "simpel." ; "ple."

	para "Vang met één van" ; "Using one of your"
	line "je #MON een" ; "#MON, catch a"

	para "insect-#MON om" ; "bug #MON to be"
	line "te beoordelen." ; "judged."

	para "Wil je het" ; "Would you like to"
	line "proberen?" ; "give it a try?"
	done

Route36NationalParkGateOfficer1GiveParkBallsText:
	text "Hier zijn de PARK-" ; "Here are the PARK"
	line "BALLEN voor het" ; "BALLS for the"
	cont "Toernooi." ; "Contest."
	done

Route36NationalParkGatePlayerReceivedParkBallsText:
	text "<PLAYER> kreeg" ; "<PLAYER> received"
	line "{d:BUG_CONTEST_BALLS} PARKBALLEN." ; "{d:BUG_CONTEST_BALLS} PARK BALLS."
	done

Route36NationalParkGateOfficer1ExplainsRulesText:
	text "De persoon met de" ; "The person who"
	line "sterkste insect-" ; "gets the strong-"
	cont "#MON is de" ; "est bug #MON"
	cont "winnaar." ; "is the winner."

	para "Je hebt {d:BUG_CONTEST_MINUTES}" ; "You have {d:BUG_CONTEST_MINUTES}"
	line "minuten." ; "minutes."

	para "Als je PARKBALLEN" ; "If you run out of"
	line "op zijn, ben je" ; "PARK BALLS, you're"
	cont "klaar." ; "done."

	para "Je kunt je laatst" ; "You can keep the"
	line "gevangen #MON" ; "last #MON you"
	cont "achteraf houden." ; "catch as your own."

	para "Ga en vang de" ; "Go out and catch"
	line "sterkste insect-" ; "the strongest bug"

	para "#MON die je" ; "#MON you can"
	line "kunt vinden!" ; "find!"
	done

Route36NationalParkGateOfficer1AskToUseFirstMonText:
	text "Uh-oh…"

	para "Je hebt meer dan" ; "You have more than"
	line "één #MON." ; "one #MON."

	para "Je moet" ; "You'll have to use"
	line "@"
	text_ram wStringBuffer3 ; MaxLength MaxPokemonNameLength
	text ", de" ; ", the"

	para "eerste #MON in" ; "first #MON in"
	line "je team gebruiken." ; "your party."

	para "Is dat oké voor" ; "Is that OK with"
	line "jou?" ; "you?"
	done

Route36NationalParkGateOfficer1WellHoldYourMonText:
	text "Prima, we bewaren" ; "Fine, we'll hold"
	line "je andere #MON" ; "your other #MON"
	cont "ondertussen." ; "while you compete."
	done

Route36NationalParkGatePlayersMonLeftWithHelperText:
	text "<PLAYER>'s #MON"
	line "zijn achtergelaten" ; "were left with the"
	cont "bij TOERNOOIHULP." ; "CONTEST HELPER."
	done

Route36NationalParkGateOfficer1ChooseMonAndComeBackText:
	text "Kies de #MON" ; "Please choose the"
	line "om te gebruiken" ; "#MON to be used"

	para "in de wedstrijd en" ; "in the Contest,"
	line "kom dan naar mij." ; "then come see me."
	done

Route36NationalParkGateOfficer1TakePartInFutureText:
	text "OK. We hopen dat" ; "OK. We hope you'll"
	line "je in de toekomst" ; "take part in the"
	cont "meedoet." ; "future."
	done

Route36NationalParkGateOfficer1FirstMonCantBattleText:
	text "Uh-oh…"
	line "De eerste #MON" ; "The first #MON"

	para "in je team kan" ; "in your party"
	line "niet vechten." ; "can't battle."

	para "Wissel met de" ; "Please switch it"
	line "#MON die je wil" ; "with the #MON"

	para "gebruiken en kom" ; "you want to use,"
	line "dan naar mij." ; "then come see me."
	done

Route36NationalParkGateOfficer1MakeRoomText:
	text "Uh-oh…"
	line "Zowel je team als" ; "Both your party"

	para "je PC-BOX zitten" ; "and your PC BOX"
	line "vol." ; "are full."

	para "Je hebt geen plek" ; "You have no room"
	line "voor de insecten-" ; "to put the bug"
	cont "die je vangt." ; "#MON you catch."

	para "Maak ruimte in je" ; "Please make room"
	line "team of PC-BOX, en" ; "in your party or"

	para "kom dan naar mij" ; "your PC BOX, then"
	line "toe." ; "come see me."
	done

Route36NationalParkGateOfficer1EggAsFirstMonText:
	text "Uh-oh…"
	line "Je hebt een EI als" ; "You have an EGG as"

	para "eerste #MON in" ; "the first #MON"
	line "je team. Wissel" ; "in your party."

	para "deze voor de" ; "Please switch it"
	line "#MON die je wil" ; "with the #MON"

	para "gebruiken en kom" ; "you want to use,"
	line "dan naar mij toe." ; "then come see me."
	done

Route36NationalParkGateOfficer1WantToFinishText:
	text "Je hebt nog @" ; "You still have @"
	text_ram wStringBuffer3 ; MaxLength 2
	text_start
	line "minuten te gaan." ; "minute(s) left."

	para "Wil je nu al" ; "Do you want to"
	line "afronden?" ; "finish now?"
	done

Route36NationalParkGateOfficer1WaitHereForAnnouncementText:
	text "OK. Wacht hier" ; "OK. Please wait"
	line "op de aankondiging" ; "here for the"

	para "van wie heeft" ; "announcement of"
	line "gewonnen." ; "the winners."
	done

Route36NationalParkGateOfficer1OkGoFinishText:
	text "OK. Ga alsjeblieft" ; "OK. Please go back"
	line "weer naar buiten" ; "outside and finish"
	cont "en rond af." ; "up."
	done

Route36NationalParkGateOfficer1ContestIsOverText:
	text "Het Toernooi van" ; "Today's Contest is"
	line "de dag is voorbij." ; "over. We hope you"

	para "Hopelijk doe je" ; "will participate"
	line "een andere keer" ; "in the future."
	cont "mee."
	done

Route36NationalParkGateOfficer1SomeMonOnlySeenInParkText:
	text "Sommige #MON" ; "Some #MON can"
	line "zijn alleen in het" ; "only be seen in"
	cont "PARK te zien." ; "the PARK."
	done

BugCatchingContestant1BText:
	text "DON: Wauw, je hebt" ; "DON: Wow, you beat"
	line "me verslagen. Je" ; "me. You're pretty"
	cont "bent best goed." ; "good."
	done

BugCatchingContestant1BStillCompetingText:
	text "DON: Geluk speelt" ; "DON: Luck plays a"
	line "een grote rol." ; "big part in this."

	para "Je weet nooit" ; "You never know"
	line "welke #MON gaan" ; "what #MON will"
	cont "verschijnen." ; "appear."
	done

BugCatchingContestant2BText:
	text "ED: Ik benijd je." ; "ED: I envy you."
	line "Ik kon het ditmaal" ; "I just couldn't"
	cont "gewoon niet." ; "do it this time."
	done

BugCatchingContestant2BStillCompetingText:
	text "ED: Misschien win" ; "ED: Maybe you win"
	line "je met grote" ; "with big #MON?"
	cont "#MON?"
	done

BugCatchingContestant3BText:
	text "NICK: Goed gedaan!" ; "NICK: Well done!"
	line "Ik ga mijn #MON" ; "I'm going to raise"
	cont "beter opvoeden." ; "my #MON better."
	done

BugCatchingContestant3BStillCompetingText:
	text "NICK: Misschien" ; "NICK: Maybe you"
	line "scoor je hoger" ; "get a higher score"

	para "met een #MON" ; "for a #MON of"
	line "met gekke kleuren." ; "an unusual color."
	done

BugCatchingContestant4BText:
	text "WILLIAM: Jij hebt" ; "WILLIAM: You're"
	line "gewonnen? Wat ving" ; "the winner? What"
	cont "je dan?" ; "did you catch?"
	done

BugCatchingContestant4BStillCompetingText:
	text "WILLIAM: Nou, ik" ; "WILLIAM: Well, I'm"
	line "ben tevreden omdat" ; "satisfied because"

	para "ik de #MON ving" ; "I caught a #MON"
	line "die ik zocht." ; "that I wanted."
	done

BugCatchingContestant5BText:
	text "BENNY: Gefeli!" ; "BENNY: Congrats!"
	line "Je hebt mijn" ; "You have earned my"
	cont "respect verdiend!" ; "respect!"
	done

BugCatchingContestant5BStillCompetingText:
	text "BENNY: Ik ving" ; "BENNY: I caught a"
	line "ooit een SCYTHER," ; "SCYTHER before,"
	cont "maar won niet." ; "but I didn't win."
	done

BugCatchingContestant6BText:
	text "BARRY: Die #MON" ; "BARRY: That #-"
	line "die je ving…" ; "MON you caught…"
	cont "Wat fantastisch!" ; "it's awesome!"
	done

BugCatchingContestant6BStillCompetingText:
	text "BARRY: Winnen is" ; "BARRY: It's easier"
	line "makkelijker met" ; "to win if you get"

	para "een insect-#MON" ; "a high-level bug"
	line "van hoog niveau." ; "#MON."

	para "Maar andere zaken" ; "But I think they"
	line "spelen geloof ik" ; "also consider some"
	cont "ook een rol." ; "other points."
	done

BugCatchingContestant7BText:
	text "CINDY: Je won?" ; "CINDY: You won?"
	line "Wat goed!" ; "That's great!"

	para "Heb je zin om met" ; "Do you feel like"
	line "mij naar insect-" ; "looking for bug"
	cont "#MON te zoeken?" ; "#MON with me?"
	done

BugCatchingContestant7BStillCompetingText:
	text "CINDY: Ik hou van" ; "CINDY: I really"
	line "insect-#MON!" ; "love bug #MON!"
	done

BugCatchingContestant8BText:
	text "JOSH: Ik… Ik kan" ; "JOSH: I… I can't"
	line "niet geloven dat" ; "believe I lost at"
	cont "ik verloor" ; "bug-catching…"
	done

BugCatchingContestant8BStillCompetingText:
	text "JOSH: Ik hoorde" ; "JOSH: I heard that"
	line "dat iemand won met" ; "somebody won with"
	cont "een CATERPIE!" ; "a CATERPIE!"
	done

BugCatchingContestant9BText:
	text "SAMUEL: Volgende" ; "SAMUEL: Next time,"
	line "keer ga ik winnen." ; "I'm going to win."
	done

BugCatchingContestant9BStillCompetingText:
	text "SAMUEL: Verdraaid." ; "SAMUEL: Darn."
	line "Ik dacht hoger te" ; "I thought I would"
	cont "scoren…" ; "score higher…"
	done

BugCatchingContestant10BText:
	text "KIPP: Kun je mij" ; "KIPP: Could you"
	line "wat tips geven?" ; "give me some tips?"

	para "Ik wil je stijl" ; "I want to study"
	line "bestuderen." ; "your style."
	done

BugCatchingContestant10BStillCompetingText:
	text "KIPP: Ik studeer" ; "KIPP: I study a"
	line "veel, maar dat is" ; "lot, but that's"

	para "onvoldoende om te" ; "not good enough to"
	line "winnen." ; "win."
	done

UnusedSilphScope2Text: ; unreferenced
; This text is referring to Sudowoodo.
; The SILPHSCOPE2 was later reworked into the SQUIRTBOTTLE.
	text "Ik hoorde dat er" ; "I hear there's a"
	line "een #MON is die" ; "#MON that looks"
	cont "op een boom lijkt." ; "just like a tree."

	para "Je kan ze identi-" ; "You can reveal its"
	line "ficeren met een" ; "identity using a"
	cont "SILPHSCOPE 2."
	done

UnusedBugCatchingContestExplanationText:
; duplicate of BugCatchingContestExplanationText in Route35NationalParkGate.asm
	text "Het KEVERTOERNOOI" ; "The Bug-Catching"
	line "is iedere dinsdag," ; "Contest is held on"

	para "donderdag en op" ; "Tuesday, Thursday"
	line "zaterdag." ; "and Saturday."

	para "Niet alleen win je" ; "Not only do you"
	line "er prijzen door" ; "earn a prize just"

	para "mee te doen, je" ; "for participating,"
	line "mag gevangen" ; "you also get to"

	para "insect-#MON" ; "keep the bug"
	line "na de wedstrijd" ; "#MON you may"

	para "ook nog eens" ; "have at the end of"
	line "houden." ; "the contest."
	done

Route36NationalParkGateOfficer1WellHoldPrizeText:
	text "Uh-oh… Je TAS" ; "Uh-oh… Your PACK"
	line "zit vol." ; "is full."

	para "We bewaren je" ; "We'll hold on to"
	line "prijs voor één" ; "your prize, but"
	cont "dag." ; "only for today."

	para "Maak wat ruimte en" ; "Please make room,"
	line "kom bij me terug." ; "then come see me."
	done

Route36NationalParkGateOfficer1HeresThePrizeText:
	text "<PLAYER>?"

	para "Hier is de prijs" ; "Here's the prize"
	line "die we voor je" ; "we were holding"
	cont "bewaarden." ; "for you."
	done

Route36NationalParkGate_MapEvents:
	db 0, 0 ; filler

	def_warp_events
	warp_event  0,  4, NATIONAL_PARK, 1
	warp_event  0,  5, NATIONAL_PARK, 2
	warp_event  9,  4, ROUTE_36, 1
	warp_event  9,  5, ROUTE_36, 2

	def_coord_events

	def_bg_events
	bg_event  6,  0, BGEVENT_READ, BugCatchingContestExplanationSign

	def_object_events
	object_event  0,  3, SPRITE_OFFICER, SPRITEMOVEDATA_STANDING_DOWN, 0, 0, -1, -1, PAL_NPC_GREEN, OBJECTTYPE_SCRIPT, 0, Route36OfficerScriptContest, EVENT_ROUTE_36_NATIONAL_PARK_GATE_OFFICER_CONTEST_DAY
	object_event  2,  5, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_RED, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant1BScript, EVENT_BUG_CATCHING_CONTESTANT_1B
	object_event  4,  5, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_GREEN, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant2BScript, EVENT_BUG_CATCHING_CONTESTANT_2B
	object_event  2,  6, SPRITE_ROCKER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_BLUE, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant3BScript, EVENT_BUG_CATCHING_CONTESTANT_3B
	object_event  6,  5, SPRITE_POKEFAN_M, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_BROWN, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant4BScript, EVENT_BUG_CATCHING_CONTESTANT_4B
	object_event  2,  7, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_RED, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant5BScript, EVENT_BUG_CATCHING_CONTESTANT_5B
	object_event  5,  6, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_GREEN, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant6BScript, EVENT_BUG_CATCHING_CONTESTANT_6B
	object_event  3,  6, SPRITE_LASS, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_BLUE, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant7BScript, EVENT_BUG_CATCHING_CONTESTANT_7B
	object_event  4,  7, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_RED, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant8BScript, EVENT_BUG_CATCHING_CONTESTANT_8B
	object_event  6,  7, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_GREEN, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant9BScript, EVENT_BUG_CATCHING_CONTESTANT_9B
	object_event  6,  6, SPRITE_YOUNGSTER, SPRITEMOVEDATA_STANDING_UP, 0, 0, -1, -1, PAL_NPC_BLUE, OBJECTTYPE_SCRIPT, 0, BugCatchingContestant10BScript, EVENT_BUG_CATCHING_CONTESTANT_10B
	object_event  3,  2, SPRITE_OFFICER, SPRITEMOVEDATA_STANDING_DOWN, 0, 0, -1, -1, PAL_NPC_GREEN, OBJECTTYPE_SCRIPT, 0, Route36NationalParkGateOfficerScript, EVENT_ROUTE_36_NATIONAL_PARK_GATE_OFFICER_NOT_CONTEST_DAY
