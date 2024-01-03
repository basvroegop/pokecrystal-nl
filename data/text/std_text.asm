NurseMornText:
	text "Goedemorgen!" ; "Good morning!"
	line "Welkom in ons" ; "Welcome to our"
	cont "#MON CENTRUM." ; "#MON CENTER."
	done

NurseDayText:
	text "Hallo!" ; "Hello!"
	line "Welkom in ons" ; "Welcome to our"
	cont "#MON CENTRUM." ; "#MON CENTER."
	done

NurseNiteText:
	text "Goedenavond!" ; "Good evening!"
	line "Je bent laat" ; "You're out late."
	cont "buiten."

	para "Welkom in ons" ; "Welcome to our"
	line "#MON CENTRUM." ; "#MON CENTER."
	done

PokeComNurseMornText:
	text "Goedemorgen!" ; "Good morning!"

	para "Dis is de #-" ; "This is the #-"
	line "MON COMMUNICATIE" ; "MON COMMUNICATION"

	para "CENTRUM--of de" ; "CENTER--or the"
	line "#COM CENTRUM." ; "#COM CENTER."
	done

PokeComNurseDayText:
	text "Hallo!" ; "Hello!"

	para "Dis is de #-" ; "This is the #-"
	line "MON COMMUNICATIE" ; "MON COMMUNICATION"

	para "CENTRUM--of de" ; "CENTER--or the"
	line "#COM CENTRUM." ; "#COM CENTER."
	done

PokeComNurseNiteText:
	text "Het is leuk je te" ; "Good to see you"
	line "zien zo laat" ; "working so late."
	line "werken."

	para "Dis is de #-" ; "This is the #-"
	line "MON COMMUNICATIE" ; "MON COMMUNICATION"

	para "CENTRUM--of de" ; "CENTER--or the"
	line "#COM CENTRUM." ; "#COM CENTER."
	done

NurseAskHealText:
	text "We zullen je #-" ; "We can heal your"
	line "MON volledig" ; #MON to perfect"
	cont "genezen." ; "health."

	para "Wil je dat we je" ; "Shall we heal your"
	line "#MON genezen?" ; "#MON?"
	done

NurseTakePokemonText:
	text "Oké, geef me je" ; "OK, may I see your"
	line "#MON even." ; "#MON?"
	done

NurseReturnPokemonText:
	text "Dank je vel voor" ; "Thank you for"
	line "het wachten." ; "waiting."

	para "Je #MON zijn" ; "Your #MON are"
	line "volledig genezen." ; "fully healed."
	done

NurseGoodbyeText:
	text "Kom snel terug!" ; "We hope to see you"
	; line "again."
	done

; not used
	text "Kom snel terug!" ; "We hope to see you"
	; line "again."
	done

NursePokerusText:
	text "Je #MON zijn" ; "Your #MON"
	line "besmet met kleine" ; "appear to be"
	line "levensvormen."

	; para "infected by tiny"
	; line "life forms."

	para "Je #MON zijn" ; "Your #MON are"
	line "gezonde en lijken" ; "healthy and seem"
	cont "goed." ; "to be fine."

	para "Maar meer weten we" ; "But we can't tell"
	line "hier in het" ; "you anything more"

	para "#MON CENTRUM" ; "at a #MON"
	line "niet." ; "CENTER."
	done

PokeComNursePokerusText:
	text "Je #MON zijn" ; "Your #MON"
	line "besmet met kleine" ; "appear to be"
	line "levensvormen."

	; para "infected by tiny"
	; line "life forms."

	para "Je #MON zijn" ; "Your #MON are"
	line "gezonde en lijken" ; "healthy and seem"
	cont "goed." ; "to be fine."

	para "Maar meer weten we" ; "But we can't tell"
	line "niet." ; "you anything more"
	done

DifficultBookshelfText:
	text "It's full of"
	line "difficult books."
	done

PictureBookshelfText:
	text "A whole collection"
	line "of #MON picture"
	cont "books!"
	done

MagazineBookshelfText:
	text "#MON magazines…"
	line "#MON PAL,"

	para "#MON HANDBOOK,"
	line "#MON GRAPH…"
	done

TeamRocketOathText:
	text "TEAM ROCKET OATH"

	para "Steal #MON for"
	line "profit!"

	para "Exploit #MON"
	line "for profit!"

	para "All #MON exist"
	line "for the glory of"
	cont "TEAM ROCKET!"
	done

IncenseBurnerText:
	text "What is this?"

	para "Oh, it's an"
	line "incense burner!"
	done

MerchandiseShelfText:
	text "Lots of #MON"
	line "merchandise!"
	done

LookTownMapText:
	text "It's the TOWN MAP."
	done

WindowText:
	text "My reflection!"
	line "Lookin' good!"
	done

TVText:
	text "It's a TV."
	done

HomepageText:
	text "#MON JOURNAL"
	line "HOME PAGE…"

	para "It hasn't been"
	line "updated…"
	done

; not used
	text "#MON RADIO!"

	para "Call in with your"
	line "requests now!"
	done

TrashCanText:
	text "There's nothing in"
	line "here…"
	done

; not used
	text "A #MON may be"
	line "able to move this."
	done

; not used
	text "Maybe a #MON"
	line "can break this."
	done

PokecenterSignText:
	text "Je #MON" ; "Heal Your #MON!"
	line "genezen!" ; "#MON CENTER"
	
	para "#MON CENTRUM"
	done

MartSignText:
	text "For All Your"
	line "#MON Needs"

	para "#MON MART"
	done

ContestResults_ReadyToJudgeText:
	text "We will now judge"
	line "the #MON you've"
	cont "caught."

	para "<……>"
	line "<……>"

	para "We have chosen the"
	line "winners!"

	para "Are you ready for"
	line "this?"
	done

ContestResults_PlayerWonAPrizeText:
	text "<PLAYER>, the No.@"
	text_ram wStringBuffer3
	text_start
	line "finisher, wins"
	cont "@"
	text_ram wStringBuffer4
	text "!"
	done

ReceivedItemText:
	text "<PLAYER> received"
	line "@"
	text_ram wStringBuffer4
	text "."
	done

ContestResults_JoinUsNextTimeText:
	text "Please join us for"
	line "the next Contest!"
	done

ContestResults_ConsolationPrizeText:
	text "Everyone else gets"
	line "a BERRY as a con-"
	cont "solation prize!"
	done

ContestResults_DidNotWinText:
	text "We hope you do"
	line "better next time."
	done

ContestResults_ReturnPartyText:
	text "We'll return the"
	line "#MON we kept"

	para "for you."
	line "Here you go!"
	done

ContestResults_PartyFullText:
	text "Your party's full,"
	line "so the #MON was"

	para "sent to your BOX"
	line "in BILL's PC."
	done

GymStatue_CityGymText:
	text_ram wStringBuffer3
	text_start
	line "#MON GYM"
	done

GymStatue_WinningTrainersText:
	text "LEADER: @"
	text_ram wStringBuffer4
	text_start
	para "WINNING TRAINERS:"
	line "<PLAYER>"
	done

CoinVendor_WelcomeText:
	text "Welcome to the"
	line "GAME CORNER."
	done

CoinVendor_NoCoinCaseText:
	text "Do you need game"
	line "coins?"

	para "Oh, you don't have"
	line "a COIN CASE for"
	cont "your coins."
	done

CoinVendor_IntroText:
	text "Do you need some"
	line "game coins?"

	para "It costs ¥1000 for"
	line "50 coins. Do you"
	cont "want some?"
	done

CoinVendor_Buy50CoinsText:
	text "Thank you!"
	line "Here are 50 coins."
	done

CoinVendor_Buy500CoinsText:
	text "Thank you! Here"
	line "are 500 coins."
	done

CoinVendor_NotEnoughMoneyText:
	text "You don't have"
	line "enough money."
	done

CoinVendor_CoinCaseFullText:
	text "Whoops! Your COIN"
	line "CASE is full."
	done

CoinVendor_CancelText:
	text "No coins for you?"
	line "Come again!"
	done

BugContestPrizeNoRoomText:
	text "Oh? Your PACK is"
	line "full."

	para "We'll keep this"
	line "for you today, so"

	para "come back when you"
	line "make room for it."
	done

HappinessText3:
	text "Wow! You and your"
	line "#MON are really"
	cont "close!"
	done

HappinessText2:
	text "#MON get more"
	line "friendly if you"

	para "spend time with"
	line "them."
	done

HappinessText1:
	text "You haven't tamed"
	line "your #MON."

	para "If you aren't"
	line "nice, it'll pout."
	done

RegisteredNumber1Text:
	text "<PLAYER> registered"
	line "@"
	text_ram wStringBuffer3
	text "'s number."
	done

RegisteredNumber2Text:
	text "<PLAYER> registered"
	line "@"
	text_ram wStringBuffer3
	text "'s number."
	done
