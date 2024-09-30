# AethaToon stencil preset guide

AethaToon has some effects possible that require your model to be set up in specific sometimes awkward ways. This will be a quick guide to setting up the "eyebrows through hair" and "fake hair shadow" effects for your avatar. I might get a little technical here, but if you can ignore the technical bits and still get it working, that's great!

---

## Eyebrows through hair

First you'll need to make sure your avatars eyes, eyebrows, etc, are own it's own submesh. This is the same as having your eyes use a separate material. In this screenshot you can see I have my eye material x_Eye_Stencil on it's own. I have many other materials on this mesh, but that doesn't matter as long as the eye material is separated (you can also have multiples of the same material, like I have here).

![Face renderer materials](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/EyesSeparateSubmesh.png)

On your eye material, in the "Render and stencil" section, select "Eye stencil" from the preset.

Next, create a material for your hair. If possible separate your hair into a "front hair" and "back hair". In my case they are separate SkinnedMeshRenderers on my model. It's OK if they're just separate materials on your renderer, but the "front hair" MUST be the last material on the list. See Submesh order below for help.

![Hair front](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/HairFront.png)

![Hair back](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/HairBack.png)

Once you're done creating your hair material select the "Hair stencil" preset.

At this point, your model should show a cutout where your eyes/eyebrows are.

Duplicate this material (ctrl-D while selected in Unity) and change it's shader to Aetha/AethaToonTransparent. Select "Hair stencil (transparent)" from the presets for this.

Finally, press the + on the renderer's materials list and add the transparent material. You should see the cutout covered up now. Change the opacity of the transparent hair material to your liking.

![Hair materials](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/TwoHairMaterials.png)

---

## Fake hair shadow

In order to get that really crisp hair shadow from your avatars bangs, I've created a small shader for writing a stencil that works with AethaToon.

Same as the guide above, your front hair should be separated as the last submesh on the renderer.

To add the shadow, create a new material with the shader Aetha/AethaSimpleShadow. Apply this material to the front hair renderer.

Make sure your avatars face is the last submesh. Create a duplicate of the face material. You'll need to tint this new material to be the "shadow" version of the face. I find this easiest by setting the directional light tint to a darker color. 

![Tinted shadow material](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/FaceShadowTint.png)

Add this new material to the same renderer as the original face material.

Finally, on the original "light" face material, select the "Fake hair shadow" stencil preset.

---

## Submesh order

When you add more materials to a renderer than there are submeshes, Unity will always render the LAST submesh on the list with the extra materials. This means for complex models where you might want a single submesh to render multiple times with multiple materials, you must somehow move it to the bottom of the list.

When exporting from Blender, the submesh order seems to be determined by the order in which materials were assigned to the mesh. I'm not 100% sure how to reliably reorder the submeshes so...

I've created (a very haphazard) script for moving the submesh up/down this list. I don't expect this tool to work 100% of the time, but if it works for you, that's great!

You can open this tool from Window -> AethaToon SubMesh Swizzle. Select a MeshRenderer/SkinnedMeshRenderer from the hierarchy. 

The window will display the list of meshes and materials. Because the submeshes are not named, the colors indicate which mesh is which. For example the red mesh will always be associated with the red buttons.

Press the up and down buttons to shift a submesh up or down in the order. By default this will also swap the appropriate materials, but you can change that with the tickbox at the top.

If you ever reimport the model, these changes will be undone.

![SubMesh Swizzle](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/StencilGuideImages/SubmeshSwizzle.png)