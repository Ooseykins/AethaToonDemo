# AethaToon guide & examples

AethaToon was designed with the intent for lights in a scene to react "realistically" with the model, but still have that toon-shade feel. If you want your model to appear lit in a different environment, the easiest way to achieve this should be to change the environment's lighting.

Some effects can be complicated, I'll try to demystify some of the new or strange material properties.

I will recommend values for some of these material properties that I consider good, like I might use on my own model. Some of these sliders and values have available ranges much greater than I would ever use, but I think it's better to give you the freedom to experiment beyond the recommended values.

---

This guide will show "before-and-after" pictures using a complete model, it can be helpful to open these two images in their own tabs, so you can click between the two to spot the differences. The scene used for these screenshots uses only a directional light and skybox for lighting. These are some screenshots of the complete model I'll reference later in the guide.

![Bust example](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/FinalLookClose.png)

![Full body example](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/FinalLookTall.png)

---

### Main texture

Regular things in this section that I would expect some familiarty of already. Cutout threshold can go below 0 and above 1, this can be helpful when animating materials appearing or disappearing in an application like Warudo. When using the transparent version of the shader there will be an opacity slider available.

Fresnel tint allows you to softly tint edges of your model, which can be used with stylize to create neat effects, such as with sheer tights. Here I show a pink tint to the model's skin, as a kind of soft subsurface-scattering effect.

[Fresnel tint OFF example](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/MainTextureFresnelOff.png)


---

### Light and shadow

This example model has shadows enabled on all renderers. I would recommend enabling/disabling cast/recieve shadows on a case-by-case basis, as well as configuring your shadow settings in-app, but for this example I thought it would be good to have them used everywhere.

You can choose to tint ambient light, shadows, directional lights, and non-directional lights separately. Non-directional lights usually means point lights but also includes spot lights and area lights. On this model I use a pink tint to shadows on the skin, which keeps it from looking gray.

[Shadow tint off](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowShadowTintOffTall.png)

Shadow blending controls the toon shading of your material.

"Start" controls where the darkest part of the shadow would start. If you imagine a realistically shaded ball, this is right in the middle, a kind of "equator". The default "realistic" value is 0.5. If you want a part of the model to appear more lit, it can help to bring this value lower, I use 0.2 on my model's face to keep it out of shadow. "Fix deep shadows" set to 1 (recommended) will fix some errors when using start values below 0.5. I never use start values above 0.5.

[Face shadow start 0.5 example](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowShadowStart0.png)

The 3 "Softness" properties change how hard or soft the edges of the shadows are. This value represents what fraction of the lit part of the model is fully lit. If you want crisp shadows: values very close to 0 (like 0.02) sometimes look better than 0 with anti-aliasing. Having multiple hard lights in a scene can look kind of weird, so I like to set my directional light to hard, and my non-directional lights to soft. Shadowmap softness can help smooth out the shape of shadows by using soft shadows on your lights, then sharpening them with this property.

[Shadow softness 1 example](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowSoftness1.png)

"Soften indirect light" will attempt to even out the ambient light colors in the scene. I recommend leaving this at 1 always. Even more I recommend using a flat color skybox for your scene if you want a strictly toon look.

[Shadow soften indirect 1, directional light off](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowDirectionalOff.png)

[Shadow soften indirect 0, directional light off](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowDirectionalOffSoftenIndirect0.png)

[Shadow soften indirect 0, directional light on](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowSoftenIndirect0.png)

"Fade shadow strength" properties let you blend shadows in/out if they're too harsh. This has a similar effect to the "Strength" setting on Unity lights.

[Fade shadow 1](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowFadeShadow1.png)

[Fade shadowmap 1](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/LightAndShadowFadeShadowmap1.png)

---

### PBR - Physically based rendering (realistic material lighting)
These settings allow you to blend a PBR material with the toon material. This is done by rendering a standard surface material and picking the brighter color per-pixel. This can help add shiny highlights to your character or make metallic parts appear more cartoonish. Works great for leather materials too.

You can preview exactly what the PBR blend looks like without the toon mixed in by setting all of your light colors to black in the "Light and Shadow" category

[PBR off](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/PBROff.png)

[PBR only](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/PBROnly.png)

PBR strength artificially boosts the PBR material to be brighter than normal. I recommend leaving this at 1, or less, for most cases.

PBR uses the same normals texture as the toon shading. In my usage I only use normals for the PBR section of the material, for adding slight bumpiness and pores to shiny skin. Leave at 0 if you don't want normals.

Metallic & smoothness are the same as they might be when using Unity's standard shader.

Catch reflections will artificially point normals in a way to amplify the amount of reflections coming from light sources. I often leave this at 0, but will sometimes pick values from 0 to 0.5.

[PBR catch reflections 0](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/PBRCatchReflections0.png)

Cubemap/ambient intensity controls how much ambient light is reflected. Depending on the situation I will use anything from 0 to 1.

[PBR cubemap intensity 0](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/PBRCubemapIntensity0.png)

Glitter effect will point normals randomly using noise. Intensity near 1 completely randomizes the direction that the normals point in, which can even be backwards. Scale is the size of the "glitter particles". Turn this effect completely off it you're not using it, it is very hard of performance.

Mask allows you to control the coverage, metallic, and smoothness of the PBR section with a texture. Red is coverage, Green is metallic, and Blue is smoothness. Metallic and smoothness of the mask texture will be multiplied by the metallic and smoothness properties, so if you're using a mask make sure to set those properties too.

---

### Outline

Outline width is defined in pixels in AethaToon, so if you get very close, or very far from the camera the outline width will remain consistent. You can use decimal values here too.

[Outline width 0](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/OutlineWidth0.png)

Outlines are always opaque and full-bright, so I recommend using black as the color for most users, unless you're absolutely sure the lighting conditions your model will be in. This is something I'd like to address in the future.

Mask allows you to hide outline in certain areas of the model. This is helpful especially if you don't want outlines around your model's mouth or other problematic areas like between the fingers or inside of elbows.

The "Fix details" option will pull or push the outlines away from the camera, which helps clear up some awkwardness and clipping issues that might crop up. Sometimes the change is very subtle. I recommend setting this to -1 in most cases, 0 in some, and never recommend above 0.

[Outline fix details 0](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/OutlineFixDetails0.png)

---

### Matcap
The matcap is very barebones. I do not use it with my own model, but I've left the option out there for "fake" reflections. PBR blending can achieve the same effect in many cases.

---

### 2D rim light

This is for adding a hard, custom rim light to your model. It's based on the depth between a pixel, and pixels offset from it. You can choose from additive blending, or alpha blending for an even more stylized effect.

[2D rim off](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/2DRimOff.png)

Main texture mix will control if the color is tinted by the main texture's color. This can look good anywhere from 0 to 1.

Minimum depth is how far in meters a pixel has to be from adjacent for the rim light to appear. I use 0.025 in most places, but you might find some other values better.

Softness will blend the rim light effect out when the depth becomes smaller. Any value from 0 to 1 can look good.

Offset is how far in meters the rim light extends into the model, measured in world space. I usually use (-0.004, -0.003, 0) to place it on the top-left of the model.

Automatic 2D rim lights will try and place the same 2D rim light effect based off of lights in the scene.

Since I use a custom rim light on my model, I usually set directional intensity to 0.

For extra-dramatic effects, you can set the intensity higher than 1, but any positive value can look nice.

[2D rim automatic](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/2DRimAutomaticBoth.png)

[2D rim only directional](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/2DRimAutomaticDirectional.png)

[2D rim only point](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/2DRimAutomaticPoint.png)

Offset is the distance in meters to offset the effect, same as above, but the direction is automatically calculated based on the position of the light.

Angle based blending will smooth out the offset when the light source moves, especially when it moves behind the model. I use this at 0.25, but any value can look OK.

---

### Rim lights

There are 2 rim lights available in AethaToon and they're definitely more confusing than ordinary rim lights (sorry!).

The direction vectors can be combined together to create smoothly blending rim lights that blend from different camera angles.

- World direction mimics a directional light, so if you want light to come down from above, set the Y component to some negative number. This vector only uses X, Y, and Z

- Camera direction is based on the up and right directions of the camera, so setting the Y component negative will seem the same as world direction negative Y, except if you roll the camera over, or tilt to extreme angles, it will maintain the effect. This vector only uses X and Y.

- View direction is "regular" rim light, based on the forward direction of the camera.

These directions blend together based on their lengths, so if you make the world vector longer, it will contribute more weight to the blending.

[Basic rim light on (view direction only)](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/RimLightOn.png)

---

### Stylized texture (halftone, dither, crosshatch)

This stylized texture can be used to blend fresnel tint, shadows, and rim lights. High softness on those properties can make this effect more clear. Scale here is set in pixels width the texture should appear as. Use square textures for this effect, there are a handful included with the shader:
- Bayer
- Crosshatch
- Tonedot
- Noise

[Stylized texture, halftone shadow](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/StylizedTextureHalftoneShadow.png)

---

### Render and stencil

This is the regular render queue and culling mode settings you might expect.

In my example I have the "eyebrows through hair" (green arrows), as well as "fake hair shadows" (blue arrows) which show as very detailed shadows of hair onto the forehead.

[Stencil presets](https://raw.githubusercontent.com/Ooseykins/AethaToonDemo/refs/heads/main/Examples/GuideImages/RenderAndStencilMisc.png)

There is a list of presets at the bottom of the material properties. You can find a usage guide here: [Stencil presets guide](https://github.com/Ooseykins/AethaToonDemo/blob/main/StencilGuide.md)
