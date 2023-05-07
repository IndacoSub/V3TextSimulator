# V3TextSimulator / DGRV3TS 

Danganronpa V3 Text Simulator.

Simulates Danganronpa V3 conversations and makes translating the game slightly easier.

Supports AI: The Somnium Files, as well (W.I.P)!

Originally based on Fire Emblem If / Fates Text Simulator ("FEFTS" or "FEITS") by SciresM: https://github.com/SciresM/FEITS

This tool is licensed under the GNU General Public License v3.0 ("GPL v3.0").

Contributions and bug/issue-reporting are welcome and appreciated.

Yes, the code is *very* messy and sometimes just... confusing.

# Folders and files

This repository does not contain any image or sound file.

You must provide those yourself.

Here are the folders where you should put your files:

All paths below are relative to the executable file.

	AI Backgrounds: Graphics\Backgrounds\AI (ex. Graphics\Backgrounds\AI\AIBlank.png)
	(TODO) AI Sprites: Graphics\Sprites\AI
	(TODO) AI Voicelines: Sound\Voices\AI

	DR Backgrounds: Graphics\Backgrounds\DR (ex. Graphics\Backgrounds\Danganronpa\default_background.png)
	DR Sprites: Graphics\Sprites\DR (ex. Graphics\Sprites\DR\C000_Saiha\anim000.png)
	DR Voicelines: Sound\Voices\DR (ex. Sound\Voices\DR\vic_Akama\vic_Akama_01_001.wav)

Each sprite folder *should* also contain a "default.png" file related to the character from the sprite(s) (will be used when no animation/expression is found).

Ideally, background images *should* be in 1280x720.

Ideally, sprites *shouldn't* have a width over 1024 pixels.

## Why it's internally called "DGRV3TS"?

The name was chosen early on in development (Early 2021) and it was never changed since then.

# License

This tool is licensed under the GPL v3.0 License.

# Credits

    “DANGANRONPA” is a registered trademark of Spike Chunsoft Co., Ltd., Too Kyo Games, LLC and NIS America Inc.
	“AI: THE SOMNIUM FILES” is a registered trademark of Spike Chunsoft Co., Ltd.
	We are not in any way affiliated or associated with them.
	
	Thanks to SciresM (https://github.com/SciresM) for creating FEITS (https://github.com/SciresM/FEITS)!
		FEITS is licensed under the GNU General Public License v3.0 ("GPL v3.0"): https://github.com/SciresM/FEITS/blob/master/LICENSE.txt
	
	This tool makes use of the following NuGet Packages:
		EPPlus 6.1.3 (https://www.epplussoftware.com), licensed under the PolyForm Noncommercial License 1.0.0: https://polyformproject.org/licenses/noncommercial/1.0.0/
		ExcelDataReader 3.6.0 (https://github.com/ExcelDataReader/ExcelDataReader), ExcelDataReader.DataSet 3.6.0 and System.Speech 7.0.0 (https://dot.net), all licensed under the MIT license: https://licenses.nuget.org/MIT
	
	The licenses related to those tools have been placed inside the ThirdParty folder.
	
	The aforementioned projects have been really useful for the development of this tool.
	
	Thanks to the members of the "SPIRAL" Discord server and the Spiral Framework project (https://spiralframework.info/ -- source code here: https://github.com/SpiralFramework/Spiral).
	
	Thanks to all of our collaborators and reviewers.