I don't like writing docs, I'm not a BA.

Requires:

Windows

.NET 4.5

Quickstart

Check the .config for log file settings. Default to c:\logs or somesuch.

Run

Set a collection name

Set a root source directory containing the collection (BattleTech/Mods will work, for example)

Use the [...] button to the right of the folder location text box. Folder Browser

Click the "Load Mods" button, bottom left of the window.

Wait.

Click the "Mod Copy" tab on the detail tab control on the right central area of the window.

Mess around.


Known Issues

1 - Don't switch tabs before loading mods. I haven't idiot proofed this yet.

2 - It takes some time to parse the mods content. 

3 - It then takes some time for WPF to set all the view dependencies for the tree. I haven't got to lazy loading yet.

4 - Selecting a node selects all children, independent of level (So selecting a mod will select everything in it's graph).

5 - This does need an instrumentation pass at some point, so don't expect stellar error handling or performance. Any error handling at all, actually :-) 

6 - It's not even in an Alpha state, so don't expect The World. I fix stuff as I come across it, and I'll refactor, tidy and implement an exception handling strategy at some point.


Notes:

Selecting stuff selects other stuff which will select other stuff (hard dependencies like mechs -> chassis -> prefab etc)

General rule is select up the dependency tree (so auto-select dependencies, not dependents. 

Item collections are handled differently: 

  They are selected when an item they contain is selected.
  
  They will only auto-select item collections they reference.
  
  They will auto-select item collections that reference them.
  
This is because when we build the custom collection, every item collection is rebuilt to only include objects resident in the sub-selection of the master list.

Untracked folders in the /mods/mymod folder are considered assets and are automatically included.

Properly parsing simgameconstants and figuring out what crap I care about is an ongoing adventure.


Required WorkArounds

1 - Custom Ammo Components. I need to add a dynamic rules system for a couple things. One is CAC. If it's in the selection (and it will be) you need to make sure ApplicationConstants is selected. Fail to do so and you will have a bad time reading output_log.txt.

2 - Dynamic Shops. Base item collection references HBS collections which I haven't accounted for yet (kurita etc). You need to ensure the following are selected: 


ItemCollection_DUMMY.csv

ItemCollection_BlackMarket.csv

ItemCollection_Smuggler.csv


When I add in the dynamic rules stuff, it will proxy select HBS faction collections which will inherit select the above, and it will just select the super special DUMMY file.


ToDos:

Lots, but I wrote PilotDef Personality. I seem to remember there were personalities I needed to associate, but I'll get there.

Contracts - Put in a prelim specialisation for contracts, ala lances. Add validation. 

IS Map/Online stuff - Eh. I don't really care about Online play or the whole IS map. Maybe never. Depends on the wind. and time.

LowVis - Considered adding a patch to interrogate all objects involved and excise optionally, needs a fair bit of investigation.

SBI - Considered adding a patch as above. eh, low on the priority list.

Need to test individual extractions.
