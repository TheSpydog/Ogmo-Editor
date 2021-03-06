# ![](https://github.com/TheSpydog/Ogmo-Editor/blob/master/OgmoEditor/Content/icons/icon32.png) Ogmo Editor: Community Edition

[Ogmo Editor](http://www.ogmoeditor.com/) is a fantastic level editor for 2D games, originally written by Matt Thorson. This fork of the project aims to improve the level editing experience by fixing longstanding bugs, adding awesome new features, and (experimentally) bringing the editor to new platforms like Mac and Linux!

## New Features

### JSON Level Exporting
When creating projects, you can now choose between XML (.oel) and JSON (.json) file exports for your levels. Choose whichever you prefer!

### Random Tile Placement
Select a rectangle of tiles from the tilemap palette, then hold Shift and click and drag to draw random tiles from your selection. Good for decals!

### Tilemap Zooming
You can now zoom in on your tile palette windows. This helps you get a better view of your assets and save precious screen real estate. Use the mouse wheel to zoom and right/middle mouse button to pan.

### Level Resizing
When resizing levels, you now have the option of expanding or shrinking from any combination of sides. Bottom/Right, Top/Left... whatever fits your needs!

### UX Improvements
Many "little things" that make your workflow smoother. Level tabs are now draggable, allowing you to re-order your levels in the editor at will. You can now edit your project without having to re-open all your levels. And the file browser now remembers the last directory you were in, resulting in fewer clicks to go where you want.

### Performance and Rendering Enhancements
Thanks to [talesofgames' ToGmo project](https://github.com/talesofgames/Ogmo-Editor), the editor is faster and smoother than ever, especially when dealing with large maps!

### Bug Fixes
Many longstanding Ogmo bugs are fixed in this fork. No more annoying exceptions or inconsistent hotkeys! 

### Experimental Mac and Linux Support (via Wine)

You can build and run this version of Ogmo Editor on macOS and Linux.

__macOS Setup:__
1. Install [Homebrew](https://brew.sh)
2. Run `brew install wine; brew install winetricks`.
3. Run `winetricks dotnet40`. Go through the installation process.

__Linux Setup:__
1. Install wine and winetricks.
2. Run `winetricks dotnet40`. Go through the installation process.

To run Ogmo Editor:

`WINEDEBUG=-all FREETYPE_PROPERTIES="truetype:interpreter-version=35" wine OgmoEditor.exe`

Note that you can only load assets from within Wine's virtual filesystem (i.e. `drive_c` and its subdirectories). This is very experimental, so please set your expectations accordingly!

### Other Stuff!
See the [changelog.html](https://github.com/TheSpydog/Ogmo-Editor/blob/master/OgmoEditor/Content/changelog.html#L50) for a list of all the fixes and additions.

## Credits

Thanks to [Matt Thorson](http://www.mattmakesgames.com/) for making Ogmo Editor originally! Thanks to talesofgames for their [ToGmo fork](https://github.com/talesofgames/Ogmo-Editor), which added some great new features and improvements!
