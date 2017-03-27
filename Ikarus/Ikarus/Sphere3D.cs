using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
        private int emptySlices = 0; //cuts slices out of the apple
        private int IndexOfFirstGeometryModel3DInModel3DGroup; //= no of lights = 2
        private Model3DGroup model3DGroup;
        private Matrix3D matrix = Matrix3D.Identity;
        private MatrixTransform3D matrixTransform3D;
        private Quaternion qXdown = new Quaternion(new Vector3D(1, 0, 0), 1); //rotations around X-axis (pitch), down direction
        private Quaternion qXup = new Quaternion(new Vector3D(1, 0, 0), -1); //rotations around X-axis (pitch), up direction
        private Quaternion qYright = new Quaternion(new Vector3D(0, 1, 0), 1); //rotations around Y-axis (yaw), right direction
        private Quaternion qYleft = new Quaternion(new Vector3D(0, 1, 0), -1); //rotations around Y-axis (yaw), left direction
        private Quaternion qZleft = new Quaternion(new Vector3D(0, 0, 1), 1); //rotations around Z-axis (roll), left direction
        private Quaternion qZright = new Quaternion(new Vector3D(0, 0, 1), -1); //rotations around Z-axis (roll), right direction
        private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        private BitmapImage Sphere3DTextur;

        public Sphere3D(Model3DGroup model3DGroupValue, BitmapImage Textur) //constructor
        {
            Sphere3DTextur = Textur;
            // model3DGroupValue is Model3DGroup from Gauges
            model3DGroup = model3DGroupValue;
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
                imageBrush = new ImageBrush(Sphere3DTextur);
                imageBrush.Viewbox = new Rect(0, lat * flatThickness, minus / Longitudes, flatThickness);
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
        { //At first delete all existing flats beginning with the last one
            SolidColorBrush backBrush = Brushes.LightGreen;
            DiffuseMaterial backMaterial = new DiffuseMaterial(backBrush);

            for (int i = model3DGroup.Children.Count - 1; i >= IndexOfFirstGeometryModel3DInModel3DGroup; i--)
                model3DGroup.Children.Remove((GeometryModel3D)model3DGroup.Children[i]);
            for (int lat = 0; lat < Latitudes - 1; lat++)
            {
                GeometryModel3D geometryModel3D = new GeometryModel3D();
                geometryModel3D.Geometry = GenerateCylinder(lat);
                geometryModel3D.Material = frontMaterial[lat];
                geometryModel3D.BackMaterial = backMaterial;
                model3DGroup.Children.Add(geometryModel3D);
            }
        }

        private MeshGeometry3D GenerateCylinder(int lat)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            for (int lon = 0; lon <= Longitudes - emptySlices; lon++)    //create a zigzaging point collection
            {
                Point3D p0 = position[lon, lat];                           //on the ceiling
                Point3D p1 = position[lon, lat + 1];                           //on the floor
                meshGeometry3D.Positions.Add(p0);                          //on the ceiling
                meshGeometry3D.Positions.Add(p1);                          //on the floor
                meshGeometry3D.Normals.Add((Vector3D)p0);                  //ceiling normal
                meshGeometry3D.Normals.Add((Vector3D)p1);                  //floor normal
                meshGeometry3D.TextureCoordinates.Add(texture[lon, lat]); //on the ceiling
                meshGeometry3D.TextureCoordinates.Add(texture[lon, lat + 1]); //on the floor
            }
            for (int lon = 1; lon < meshGeometry3D.Positions.Count - 2; lon += 2)
            { //first triangle = left upper part of a rectangle
                meshGeometry3D.TriangleIndices.Add(lon - 1); //left  upper point
                meshGeometry3D.TriangleIndices.Add(lon); //left  lower point
                meshGeometry3D.TriangleIndices.Add(lon + 1); //right upper point
                                                             //second triangle = right lower part of the rectangle
                meshGeometry3D.TriangleIndices.Add(lon + 1); //right upper point
                meshGeometry3D.TriangleIndices.Add(lon); //left  lower point
                meshGeometry3D.TriangleIndices.Add(lon + 2); //right lower point
            }
            return meshGeometry3D;
        }

        Matrix3D CalculateRotationMatrix(double x, double y, double z) // in degrees
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix, y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }

        public void Rotate(double x, double y, double z) // in degrees
        {
            matrixTransform3D = new MatrixTransform3D(CalculateRotationMatrix(x, y, z));

            for (int i = IndexOfFirstGeometryModel3DInModel3DGroup; i < model3DGroup.Children.Count; i++)
                ((GeometryModel3D)model3DGroup.Children[i]).Transform = matrixTransform3D;
        }
    }
}

