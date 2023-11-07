# MipmapStreaming

非常方便的 MipmapStreaming 方案，自动优化不可见物体的纹理显存



制作流程



1. 纹理需要生成 Mipmap 并且支持 MipmapStreaming

![image](https://github.com/kuronekoyang/MipmapStreaming/assets/79456057/a9ca1ef3-b2f0-4656-b5c8-21940c9ca29e)



2. 将 MipmapStreamingManager 添加到场景中

![image](https://github.com/kuronekoyang/MipmapStreaming/assets/79456057/9111c104-d11c-46d2-ae39-378000755a7f)



3. 将 MipmapStreamingController 添加到游戏中的物体

![image](https://github.com/kuronekoyang/MipmapStreaming/assets/79456057/094fbd16-d5ce-479c-949c-18ea67a28e82)



如何查看效果

1. 启动测试场景

2. 将 TestMeshRenderer 移到摄像机外，或者将 TestParticleSystem 隐藏，都可以看到Log有相应的输出，纹理的Mipmap等级被修改（数字越大显存占用越小）

3. 如果想查看实际显存占用大小，请打包测试，在Editor环境下测试存在误差
