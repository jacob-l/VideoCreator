using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Configuration;
using CCP.Core.Nodes;
using CCP.Core.Jobs;
using CCP.Core.Messaging;
using CCP.Core.Messaging.Factories;
using CCP.Core.Utils;
using CCP.Extensions.SelectStrategies.XPathStrategy;
using CCP.Extensions.Endpoints.WCF.TCP;
using eOculus.Core.Cameras;
using eOculus.Core.GraphicsObjects;
using eOculus.Core.Lights;
using eOculus.Core.Scenes;
using eOculus.Core.Utils;
using eOculus.Primitives;
using Core;
using Core.Coders;
using Core.Rendering;
using Tests.Coders;
using Tests.DataPrepare;

namespace VideoCreator
{
    class Program
    {
        static String StartTasks(Node node)
        {
            String main = "main" + Guid.NewGuid().ToString();
            node.AddTask(main);
            String localTasks = ConfigurationManager.AppSettings["thisTasks"];
            String[] tasks = localTasks.Split(' ');
            for (int i = 0; i < tasks.Length; i++ )
                node.AddTask(tasks[i]);

            Console.WriteLine("StartedTasks");
            return "/" + node.Name + "/" + main;
        }

        private static Scene CreateScene3()
        {
            Size resolution = new Size(640, 480);
            Point3D position = new Point3D(0, 0, 5000);
            double focus = 1000;
            SizeF projectionWindow = new SizeF(200, 150);
            Rotation rotation = new Rotation(-Math.PI * 0.25, 0, -Math.PI / 2.3);
            PerspectiveCamera camera = new PerspectiveCamera(position, rotation, projectionWindow, resolution, focus);

            Scene scene = new Scene();
            scene.Camera = camera;
            scene.InfinityColor = Color.Black;

            Material mS1 = new Material(Color.Green, 0.3, 0.3, 0.6, 0, 5);
            Material mS2 = new Material(Color.Red, 0.4, 0.4, 0, 0.4, 5);
            Material mS3 = new Material(Color.Yellow, 0.4, 0.3, 0.5, 0, 5);
            Material mS4 = new Material(Color.BlueViolet, 0.4, 0.3, 0.4, 0, 5);

            Material mC1 = new Material(Color.Black, 0.3, 0.3, 0.4, 0, 5);
            Material mC2 = new Material(Color.White, 0.3, 0.3, 0.4, 0, 5);

            Carpet carpet = new Carpet(new Point3D(0, 0, 0), new SizeF(5000, 5000), new Size(50, 50), mC1, mC2);
            double r = 100;
            double h = 2 * r * 0.866;
            double x = -h * 0.66 * 0.7;
            //шары в форме пирамиды
            Sphere sphere1 = new Sphere(new Point3D(x, x, r), r, mS1);
            Sphere sphere2 = new Sphere(new Point3D(x + h * 0.7 - r * 0.7, x + h * 0.7 + r * 0.7, r), r, mS2);
            Sphere sphere3 = new Sphere(new Point3D(x + h * 0.7 + r * 0.7, x + h * 0.7 - r * 0.7, r), r, mS3);
            Sphere sphere4 = new Sphere(new Point3D(x + h * 0.7 * 0.66, x + h * 0.7 * 0.66, 2.5 * r), r, mS4);

            scene.AddGraphicsObject(carpet);

            scene.AddGraphicsObject(sphere1);
            scene.AddGraphicsObject(sphere2);
            scene.AddGraphicsObject(sphere3);
            scene.AddGraphicsObject(sphere4);

            Light L1 = new Light(new Point3D(0, 0, 2000), Color.White);
            Light L2 = new Light(new Point3D(0, 2000, 3000), Color.White);
            Light L3 = new Light(new Point3D(2000, 0, 3000), Color.White);
            Light L4 = new Light(new Point3D(0, -2000, 3000), Color.White);
            Light L5 = new Light(new Point3D(-2000, 0, 3000), Color.White);
            //scene.AddLight(L1);
            scene.AddLight(L2);
            scene.AddLight(L3);
            scene.AddLight(L4);
            scene.AddLight(L5);

            return scene;
        }

        static Node ConfigureNode()
        {
            String json = ConfigurationManager.AppSettings["nodeConfig"];
            NodeConfig config = NodeConfigFactory.Create(json);
            Node node = new Node(config);
            node.Start();
            Thread.Sleep(100);
            return node;
        }

        static ICoder GetCoder()
        {
            String processName = ConfigurationManager.AppSettings["ffmpeg"];
            return new CoderFfmpeg(processName);
        }

        static IRender GetRender(int quantity)
        {
            List<ICamera> camersList = new List<ICamera>();
            
            for(int i = 0; i < quantity; i++)
            {
                Size resolution = new Size(640, 480);
                Point3D position = new Point3D(0, 0, 5000);
                double focus = 1000;
                SizeF projectionWindow = new SizeF(200, 150);
                double aroundOZ = -(Math.PI * 2.0) * (((double)i) / quantity);
                Rotation rotation = new Rotation(aroundOZ, 0, -Math.PI / 2.3);
                PerspectiveCamera camera = new PerspectiveCamera(position, rotation, projectionWindow, resolution, focus);
                camersList.Add(camera);
            }
            return new EOculusRender(CreateScene3(), new Size(800, 600), camersList.GetEnumerator());
        }

        static void TestDebug()
        {
        }

        static void Main(string[] args)
        {
            InputEndpointFactoryManager.Instance.Register(new InputWCFTCPEndpointFactory());
            OutputEndpointFactoryManager.Instance.Register(new OutputWCFTCPEndpointFactory());

            Node node = ConfigureNode();
            String main = StartTasks(node);

            List<String> workingTasks = new List<string>(ConfigurationManager.AppSettings["usingTasks"].Split(' '));

            ICoder coder = GetCoder();
            int quantity = int.Parse(ConfigurationManager.AppSettings["quantity"]);

            Console.WriteLine("Press enter to start manage work.");
            Console.ReadKey();
            if (quantity == 0)
                return;
            Object[] arguments = new Object[6];
            arguments[0] = "Manager" + Guid.NewGuid().ToString();
            arguments[1] = workingTasks;
            arguments[2] = GetRender(quantity);
            arguments[3] = quantity;
            arguments[4] = coder;
            arguments[5] = "out.avi";
            Message msg = new AddJobGenericMessage<ManagerVideoJob>(arguments);
            msg.Target = new XPathSelectStrategy(main);
            node.Router.Send(msg);
        }
    }
}
