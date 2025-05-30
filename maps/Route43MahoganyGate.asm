	object_const_def
	const ROUTE43MAHOGANYGATE_OFFICER

Route43MahoganyGate_MapScripts:
	def_scene_scripts

	def_callbacks

Route43MahoganyGateOfficer:
	faceplayer
	opentext
	checkevent EVENT_CLEARED_ROCKET_HIDEOUT
	iftrue .RocketsCleared
	writetext Route43MahoganyGateOfficerText
	waitbutton
	closetext
	end

.RocketsCleared:
	writetext Route43MahoganyGateOfficerRocketsClearedText
	waitbutton
	closetext
	end

Route43MahoganyGateOfficerText:
	text "Alleen mensen op" ; "Only people headed"
	line "weg naar MEER VAN" ; "up to LAKE OF RAGE"

	para "RAZERNIJ passeren" ; "have been through"
	line "laatste tijd hier." ; "here lately."
	done

Route43MahoganyGateOfficerRocketsClearedText:
	text "Niemand gaat nog" ; "Nobody goes up to"
	line "naar het MEER VAN" ; "LAKE OF RAGE these"
	cont "RAZERNIJ." ; "days."
	done

Route43MahoganyGate_MapEvents:
	db 0, 0 ; filler

	def_warp_events
	warp_event  4,  0, ROUTE_43, 1
	warp_event  5,  0, ROUTE_43, 2
	warp_event  4,  7, MAHOGANY_TOWN, 5
	warp_event  5,  7, MAHOGANY_TOWN, 5

	def_coord_events

	def_bg_events

	def_object_events
	object_event  0,  4, SPRITE_OFFICER, SPRITEMOVEDATA_STANDING_RIGHT, 0, 0, -1, -1, PAL_NPC_RED, OBJECTTYPE_SCRIPT, 0, Route43MahoganyGateOfficer, -1
