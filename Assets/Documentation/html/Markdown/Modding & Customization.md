# Modding

MAKE HTML ANCHORS FOR THESE SO I CAN REFERENCE THEM LATER.

## Useful terms & Info
### **Piano vs Keyboard**
<div style="margin-left: 40px;">
	While the name of the game -KiBoard- is in reference to a musical electronic keyboard. For the sake of simplicity throughout this document, all instances of the word keyboard in this document will be in reference to a computer keyboard unless specified elsewise. Any reference to a musical keyboard will be referred to as a piano
	</div>
### General Modding Tips

<div style="margin-left: 40px;">
	Files are found in **%AppData%/LocalLow/MightBMay/KiBoard**.<br><br> 
	  
	To get to the appdata folder, press Windows key + R, and type **%AppData%** ( with the % signs). This will open **%AppData%/Roaming/**. Go up one folder and navigate to **%AppData%/LocalLow/MightBMay/KiBoard**.<br><br> 
	  
	Anywhere you see it in the document, replace words surrounded by { } with your input, whether it be the name of your song or anything else.<br><br> 
	  
	Some files are song specific- for example a songs icon- and some are not, like the background for the song selection screen.<br><br> 
	
	Anything song specific will go in that specific songs folder in **“.../KiBoard/Songs/”**. Anything else must go in the **“.../KiBoard/Images“** folder.<br><br> 
	</div>
## Custom Songs
### Creating a custom song
<div style="margin-left: 40px;">
	Go to %appdata%/LocalLow/MightBMay/KiBoard/Songs. Create a folder with the name of your song.<br><br> 
	
	You must name files in this folder with the same name as the folder. Capitals do not matter, but check your spelling.<br><br> 
	
	Place your .mid file and .mp3 file in the created folder.  
	You can now open the game or reload the song selection screen and play your song.<br><br> 
	  
	
	The game automatically reads the midi file and extracts the notes from it, and saves it to a compressed JSON file in that path.<br><br> 
	
	</div>
### Dressing a custom song
<div style="margin-left: 40px;">
Songs have a few customization options. 
** FINISH LATER: MAKE LINK TO SUPPORTED FILE TYPES AND CUSTOM SONG IMAGE TAGS.**

You can add images to your song by adding a **supported image file type** to the custom song folder with one of the listed **Custom Song Image Tags**
</div>
### Other additions
<div style="margin-left: 40px;">
#### Multiple Versions of Songs:
<div style="margin-left: 40px;">
	You can add multiple difficulties or versions to a song by adding multiple Midi files.  
	Differentiate from the versions by adding a version name to the file name. If you want 
	to use a different audio file, you must make a separate custom song.<br><br> 
	
	  
	Add  {version name} after the name of the song. For example with the song FurElise,  
	you could name the more difficult version FurElise_Hard.mid.<br><br>
	</div>
</div>

# Customization

## Custom Song Image Tags

Custom Song Image Tags are used to assign images to varying parts of the song. Remember that the file name must also be the same as the custom song's folder name. For example, to change the icon of the song FurElise, you would name your file `icon_FurElise.png`.
	<div style="margin-left: 40px;">
	| Tag   | Description                                     |
	|-------|-------------------------------------------------|
	| bg_   | Changes the background image when playing the song. |
	| icon_ | Changes the song icon in song selection.        |
	</div><br><br>
## UI Image Tags

UI Image Tags are used to assign images to certain elements of the players UI in many scenes of the game. Each of the following names are split by their in-game scene, which must prefix their desired UI Element. For example, the background of the song selection screen must be named `SongSelection_Background.png`.
<div style="margin-left: 40px;">

### <b>SongSelection_</b>
- Background: The background of the song selection screen.

### <b>blank_</b>
- tag: Description.

</div><br><br>