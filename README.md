# InfiniteForgeXMLReplacer
 An XML editor to mass replace objects, material and grime inside halo infinite forge's maps but all ids are converted to a name equivilent.

---

Replace for retail currently:
- will delete your folder structure to allow objects to be renamed, this will not affect any of your objects in any way but visual (it will remove any folders you have set up) this will cause your game to have to regenerate any data structure, so it may take a while to load the first time.
- Changes all forerunner primitives to their generic equivalent.
- Changes all FX objects into pointers.
- Changes all grass objects into pointers.
- Changes as many MP blocks as possible into non-MP as possible.

Replace object currently:
- Allows you to replace 1 object type at a time to another, E.G PRIMITIVE_CUBE to FORERUNNER_CUBE

Material and Grime replacement does not check if the swap is a valid one, check the options exist in game.

Replace material currently:
- Allows you to replace 1 material type at a time to another, E.G BRUSHED to DESIGN_GRID

Replace grime currently:
- Allows you to replace 1 grime type at a time to another, E.G ALGAE to ASPHALT

---

Instructions:
- Run the application
- Load mvar xml (if you do not have an xml file of your map then unpack it using halo infinite variant tool)
- Replace your objects/materials or replace for retail
- Repack your mvar XML with infinite variant tool
- load as normal
