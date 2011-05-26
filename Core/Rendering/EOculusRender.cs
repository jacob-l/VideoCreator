using System;
using System.Collections.Generic;
using System.Drawing;
using eOculus.Core.Scenes;
using eOculus.Core.Cameras;
using eOculus.Core.Rendering;

namespace Core.Rendering
{
    [Serializable]
    public class EOculusRender:IRender
    {
        List<ICamera> listCameras = new List<ICamera>();
        Scene scene;
        Size frameSize;

        public EOculusRender(Scene scene, Size frameSize, IEnumerator<ICamera> iterCameras)
        {
            iterCameras.Reset();
            while (iterCameras.MoveNext())
                listCameras.Add(iterCameras.Current);
            this.scene = scene;
            this.frameSize = frameSize;
        }

        public Bitmap GetBitmap(int i)
        {
            Renderer renderer = new Renderer();
            scene.Camera = listCameras[i];
            return scene.Draw(renderer, frameSize);
        }
    }
}
