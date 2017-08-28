using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

/*
        / This Bloch copy to gauges XAML file, fefore the canvas elements...
        <!--Viewport3D is a drawing canvas which resizes its Content automatically-->
        <Viewport3D x:Name="viewport" Margin="54,52,50,78">
            <Viewport3D.Camera>
                <PerspectiveCamera x:Name="perspectiveCamera" Position=" 0 0 2.5" LookDirection=" 0 0 -1" UpDirection=" 0 1 0"/>
            </Viewport3D.Camera>
            <!--Any 3D-content must be packed in a ModelVisual3D-object-->
            <ModelVisual3D>
                <ModelVisual3D.Content>
                    <!--Only one Content is allowed. Thus we use a Model3DGroup as envelope for our two lights and all further GeometryModel3Ds.-->
                    <Model3DGroup x:Name="model3DGroup">
                        <AmbientLight Color="#444444"/>
                        <DirectionalLight x:Name="directionalLight" Color="#ffffff" Direction="-1 -1 -1" />
                        <!--A lot of GeometryModel3Ds will be inserted here.-->
                    </Model3DGroup>
                </ModelVisual3D.Content>
            </ModelVisual3D>
        </Viewport3D>
*/

namespace Ikarus
{
    class Sphere3D
    {
        private const int Longitudes = 30; // 100 maximum
        private const int Latitudes = 30; // 100 maximum
        private Point3D[,] position = new Point3D[Longitudes + 1, Latitudes];
        private Point[,] texture = new Point[Longitudes + 1, Latitudes];
        private DiffuseMaterial[] frontMaterial = new DiffuseMaterial[Latitudes - 1];
        private int emptySlices = 0;                            //cuts slices out of the apple
        private int IndexOfFirstGeometryModel3DInModel3DGroup;  //= no of lights = 2
        private Model3DGroup model3DGroup;
        private Matrix3D matrix = Matrix3D.Identity;
        private MatrixTransform3D matrixTransform3D;
        private BitmapImage sphere3DTextur;
        private Matrix3D matrix3D = new Matrix3D();

        public  double xRotation = 0.0;
        public double yRotation = 0.0;
        public double zRotation = 0.0;

        public Sphere3D(Model3DGroup _model3DGroup, BitmapImage _textur) //constructor
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            sphere3DTextur = _textur;
            
            // model3DGroupValue is Model3DGroup from Gauges
            model3DGroup = _model3DGroup;
            IndexOfFirstGeometryModel3DInModel3DGroup = model3DGroup.Children.Count;

            GenerateImageMaterials();
            GenerateSphere(Longitudes, Latitudes);
            GenerateAllCylinders();

            /*
            light_move_slider.Value = -180 to 180
            double arcus = 2.0 * Math.PI * light_move_slider.Value / 360;
            Vector3D v = new Vector3D();
            v.X = -3.0 * Math.Sin(arcus);
            v.Y = 0;
            v.Z = -3.0 * Math.Cos(arcus);
            directionalLight.Direction = v;
            */
        }

        private void GenerateImageMaterials()
        {
            ImageBrush imageBrush;
            double flatThickness = 1.0 / (Latitudes - 1);
            double minus = (double)(Longitudes - emptySlices);

            for (int lat = 0; lat < Latitudes - 1; lat++)
            {
                imageBrush = new ImageBrush(sphere3DTextur)
                {
                    Viewbox = new Rect(0, lat * flatThickness, minus / Longitudes, flatThickness)
                };
                frontMaterial[lat] = new DiffuseMaterial(imageBrush);
            }
        }

        private void GenerateSphere(int longitudes, int latitudes)
        {
            double latitudeArcusIncrement = Math.PI / (latitudes - 1);
            double longitudeArcusIncrement = 2.0 * Math.PI / longitudes;

            for (int lat = 0; lat < latitudes; lat++)
            {
                double latitudeArcus = lat * latitudeArcusIncrement;

                double radius = Math.Sin(latitudeArcus);
                //if ( lat == latitudes/2 ) radius *= 1.3;
                double y = Math.Cos(latitudeArcus);
                double textureY = (double)lat / (latitudes - 1);
                for (int lon = 0; lon <= longitudes; lon++)
                {
                    double longitudeArcus = lon * longitudeArcusIncrement;
                    position[lon, lat].X = radius * Math.Cos(longitudeArcus);
                    position[lon, lat].Y = y;
                    position[lon, lat].Z = -radius * Math.Sin(longitudeArcus);
                    texture[lon, lat].X = (double)lon / longitudes;
                    texture[lon, lat].Y = textureY;
                }
            }
        }

        private void GenerateAllCylinders()
        {                                                                        //At first delete all existing flats beginning with the last one
            SolidColorBrush backBrush = Brushes.LightGreen;
            DiffuseMaterial backMaterial = new DiffuseMaterial(backBrush);

            for (int i = model3DGroup.Children.Count - 1; i >= IndexOfFirstGeometryModel3DInModel3DGroup; i--)
                model3DGroup.Children.Remove((GeometryModel3D)model3DGroup.Children[i]);

            for (int lat = 0; lat < Latitudes - 1; lat++)
            {
                GeometryModel3D geometryModel3D = new GeometryModel3D()
                {
                    Geometry = GenerateCylinder(lat),
                    Material = frontMaterial[lat],
                    BackMaterial = backMaterial
                };
                model3DGroup.Children.Add(geometryModel3D);
            }
        }

        private MeshGeometry3D GenerateCylinder(int lat)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

            for (int lon = 0; lon <= Longitudes - emptySlices; lon++)           // create a zigzaging point collection
            {
                Point3D p0 = position[lon, lat];                                // on the ceiling
                Point3D p1 = position[lon, lat + 1];                            // on the floor
                meshGeometry3D.Positions.Add(p0);                               // on the ceiling
                meshGeometry3D.Positions.Add(p1);                               // on the floor
                meshGeometry3D.Normals.Add((Vector3D)p0);                       // ceiling normal
                meshGeometry3D.Normals.Add((Vector3D)p1);                       // floor normal
                meshGeometry3D.TextureCoordinates.Add(texture[lon, lat]);       // on the ceiling
                meshGeometry3D.TextureCoordinates.Add(texture[lon, lat + 1]);   // on the floor
            }
            for (int lon = 1; lon < meshGeometry3D.Positions.Count - 2; lon += 2)
            {                                                                   // first triangle = left upper part of a rectangle
                meshGeometry3D.TriangleIndices.Add(lon - 1);                    // left  upper point
                meshGeometry3D.TriangleIndices.Add(lon);                        // left  lower point
                meshGeometry3D.TriangleIndices.Add(lon + 1);                    // right upper point
                                                                                // second triangle = right lower part of the rectangle
                meshGeometry3D.TriangleIndices.Add(lon + 1);                    // right upper point
                meshGeometry3D.TriangleIndices.Add(lon);                        // left  lower point
                meshGeometry3D.TriangleIndices.Add(lon + 2);                    // right lower point
            }
            return meshGeometry3D;
        }

        private Matrix3D CalculateRotationMatrix() // in degrees
        {
            matrix3D = new Matrix3D();

            //matrix3D.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));                  // this is for a 3D Object in a 3D world.
            //matrix3D.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix3D, y));
            //matrix3D.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix3D, z));

            matrix3D.Rotate(new Quaternion(new Vector3D(1, 0, 0), xRotation));                  // and this is for a ADI. The mechanic is difference.
            matrix3D.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix3D, yRotation));
            matrix3D.Rotate(new Quaternion(new Vector3D(0, 0, 1), zRotation));

            return matrix3D;
        }

        //public void Rotate(double x, double y, double z) // in degrees
        //{
        //    xRotation = x;
        //    yRotation = y;
        //    zRotation = z;

        //    matrixTransform3D = new MatrixTransform3D(CalculateRotationMatrix());

        //    for (int i = IndexOfFirstGeometryModel3DInModel3DGroup; i < model3DGroup.Children.Count; i++)
        //        ((GeometryModel3D)model3DGroup.Children[i]).Transform = matrixTransform3D;
        //}

        //public void Rotate(ref double x, ref double y, ref double z) // in degrees
        //{
        //    xRotation = x;
        //    yRotation = y;
        //    zRotation = z;

        //    matrixTransform3D = new MatrixTransform3D(CalculateRotationMatrix());

        //    for (int i = IndexOfFirstGeometryModel3DInModel3DGroup; i < model3DGroup.Children.Count; i++)
        //        ((GeometryModel3D)model3DGroup.Children[i]).Transform = matrixTransform3D;
        //}

        public void Rotate() // in degrees
        {
            matrixTransform3D = new MatrixTransform3D(CalculateRotationMatrix());

            for (int i = IndexOfFirstGeometryModel3DInModel3DGroup; i < model3DGroup.Children.Count; i++)
                ((GeometryModel3D)model3DGroup.Children[i]).Transform = matrixTransform3D;
        }
    }
}

