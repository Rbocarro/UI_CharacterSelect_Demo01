
# Character Selection Demo
![](./ReadmeAssets/UIDemo_01.png)
For this Project, I set decided to make a UI character selection demo for a concept of a multiplayer game inspired by games such as Marvel Rivals and Valorant, being mainly a programmer, i wanted to implement as much of the UI animations as i could procedurally as opposed to keyframing and animation UI elements using external tools like

# Project Web Demo
[Web Demo- PC recommended](https://rbocarro.github.io/UIDemo02/)


# Tools Used


## PrimeTween
This project untilises PrimeTween for handling all UI animations and transitions. I evaluated it against other tweening libraries such as DOTween, LeanTween and found PrimeTween to offer better performance. It allows for  easy chaining of tweens and sequencing of animations without additional overhead. 

#### Link:
[PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)


## ScriptableObject
![](./ReadmeAssets/ScriptableObject.png)
Unity’s ScriptableObject system was used to manage character data, including abilities, artwork, and profile information. This allows game designers to create and modify character data directly in the Unity Editor without altering code, promoting a clear separation between data and logic, making the system highly extensible and designer-friendly.

## Shader Graph
![](./ReadmeAssets/shaderGraph01.png)
.


## Improvements
If given more time, This project could benefit from improvements such as cleaning up code structure to improve readability and better compartmentalised functions.

+ Calculation of Simplex noise is expensive, especially in 3 dimensions so steps could be made to reduce computation reqirement such as using a prerendered video of the noise animation.












