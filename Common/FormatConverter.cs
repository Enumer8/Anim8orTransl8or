using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using Anim8orTransl8or.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Anim8orTransl8or
{
   public static class FormatConverter
   {
      public static COLLADA Convert(ANIM8OR an8)
      {
         COLLADA dae = new COLLADA();
         CreateAsset(dae, an8);
         CreateLibraryCameras(dae, an8);
         CreateLibraryImages(dae, an8);
         CreateLibraryEffects(dae, an8);
         CreateLibraryMaterials(dae, an8);
         CreateLibraryGeometries(dae, an8);
         CreateLibraryAnimations(dae, an8);
         CreateLibraryControllers(dae, an8);
         CreateLibraryVisualScenes(dae, an8);
         CreateScene(dae, an8);
         return dae;
      }

      #region common
      static readonly Char[] NUM_CHARS = new Char[]
         { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

      static String MakeUnique(String name)
      {
         Int64 number = 0;

         name = name ?? "Unnamed";

         while ( sNames.Contains(name) )
         {
            name = $"{name.TrimEnd(NUM_CHARS)}{++number:00}";
         }

         sNames.Add(name);
         return name;
      }
      #endregion

      #region asset
      static void CreateAsset(COLLADA dae, ANIM8OR an8)
      {
         AssemblyName assembly = typeof(FormatConverter).Assembly.GetName();

         dae.asset = new asset();
         dae.asset.contributor = new assetContributor[1];
         dae.asset.contributor[0] = new assetContributor();
         dae.asset.contributor[0].author = "Anim8or v" +
            an8?.header?.version?.text ?? "0.0.0" + " build " +
            an8?.header?.build?.text ?? "1970.1.1";
         dae.asset.contributor[0].authoring_tool =
            assembly.Name + " v" + assembly.Version.ToString(3);
         dae.asset.contributor[0].comments =
            String.Join("\r\n", an8?.description?.text ?? new String[0]);
         dae.asset.created = DateTime.Now;
         dae.asset.modified = dae.asset.created;
         dae.asset.unit = new assetUnit();
         dae.asset.unit.name = "meter";
         dae.asset.unit.meter = 1;
         dae.asset.up_axis = UpAxisType.Y_UP;
      }
      #endregion

      #region library_cameras
      static void CreateLibraryCameras(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
      }
      #endregion

      #region library_images
      static void CreateLibraryImages(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
      }
      #endregion

      #region library_effects
      static void CreateLibraryEffects(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
      }
      #endregion

      #region library_materials
      static void CreateLibraryMaterials(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
      }
      #endregion

      #region library_geometries
      static void CreateLibraryGeometries(COLLADA dae, ANIM8OR an8)
      {
         library_geometries library = new library_geometries();
         dae.Items = dae.Items.Append(library);

         foreach ( @object o in an8?.@object ?? new @object[0] )
         {
            // Convert the object to a group
            group2 g = new group2();
            g.name = new @string() { text = o.name };
            g.mesh = o.mesh;
            g.sphere = o.sphere;
            g.cylinder = o.cylinder;
            g.cube = o.cube;
            g.subdivision = o.subdivision;
            g.pathcom = o.pathcom;
            g.textcom = o.textcom;
            g.modifier = o.modifier;
            g.image = o.image;
            g.group = o.group;

            AddGroup(library, sVisualNode, g, o);
         }

         foreach ( figure f in an8?.figure ?? new figure[0] )
         {
            if ( f.bone != null )
            {
               bone2 b = new bone2();
               // Use the figure name for convenience
               b.name = f.name;
               // Note: Anim8or v1.00 ignores the root bone's length
               b.length = null;
               b.diameter = f.bone.diameter;
               b.orientation = f.bone.orientation;
               b.locked = f.bone.locked;
               b.dof = f.bone.dof;
               b.influence = f.bone.influence;
               b.mesh = f.bone.mesh;
               b.sphere = f.bone.sphere;
               b.cylinder = f.bone.cylinder;
               b.cube = f.bone.cube;
               b.subdivision = f.bone.subdivision;
               b.pathcom = f.bone.pathcom;
               b.textcom = f.bone.textcom;
               b.modifier = f.bone.modifier;
               b.image = f.bone.image;
               b.group = f.bone.group;
               b.namedobject = f.bone.namedobject;
               b.bone = f.bone.bone;

               AddBoneGeometry(an8, library, sVisualNode, b, f.bone);
            }
         }
      }

      static VisualNode AddGroup(
         library_geometries library,
         VisualNode parent,
         group2 g,
         Object tag)
      {
         String name = MakeUnique(g.name?.text);
         point origin = g.@base?.origin?.point ?? new point();
         quaternion orientation = g.@base?.orientation?.quaternion ??
            new quaternion(0, 0, 0, 1);
         Double[] matrix = orientation.ToMatrix(origin);
         VisualNode node = new VisualNode(name, matrix, tag);
         node.Parent = parent;
         parent.Children.Add(node);

         foreach ( An8.V100.mesh m in g.mesh ?? new An8.V100.mesh[0] )
         {
            AddMesh(library, node, m, m);
         }

         foreach ( An8.V100.sphere s in g.sphere ??
                   new An8.V100.sphere[0] )
         {
            AddMesh(library, node, An8Sphere.Convert(s), s);
         }

         foreach ( An8.V100.cylinder c in g.cylinder ??
                   new An8.V100.cylinder[0] )
         {
            AddMesh(library, node, An8Cylinder.Convert(c), c);
         }

         foreach ( cube c in g.cube ?? new cube[0] )
         {
            AddMesh(library, node, An8Cube.Convert(c), c);
         }

         foreach ( subdivision s in g.subdivision ?? new subdivision[0] )
         {
            AddMesh(library, node, An8Subdivision.Convert(s), s);
         }

         foreach ( pathcom p in g.pathcom ?? new pathcom[0] )
         {
            AddMesh(library, node, An8PathCom.Convert(p), p);
         }

         foreach ( textcom t in g.textcom ?? new textcom[0] )
         {
            AddMesh(library, node, An8TextCom.Convert(t), t);
         }

         foreach ( modifier2 m in g.modifier ?? new modifier2[0] )
         {
            // Convert the modifier to a group
            // Note: The modifier's base/pivot only affects the modifier.
            group2 g2 = new group2();
            g2.name = m.name;
            g2.mesh = g2.mesh.Append(m.mesh);
            g2.sphere= g2.sphere.Append(m.sphere);
            g2.cylinder = g2.cylinder.Append(m.cylinder);
            g2.cube= g2.cube.Append(m.cube);
            g2.subdivision = g2.subdivision.Append(m.subdivision);
            g2.pathcom = g2.pathcom.Append(m.pathcom);
            g2.textcom = g2.textcom.Append(m.textcom);
            g2.modifier = g2.modifier.Append(m.modifier);
            g2.image = g2.image.Append(m.image);
            g2.group = g2.group.Append(m.group);

            AddGroup(library, node, g2, m);
         }

         foreach ( An8.V100.image i in g.image ?? new An8.V100.image[0] )
         {
            AddMesh(library, node, An8Image.Convert(i), i);
         }

         foreach ( group2 g2 in g.group ?? new group2[0] )
         {
            AddGroup(library, node, g2, g2);
         }

         return node;
      }

      static VisualNode AddMesh(
         library_geometries library,
         VisualNode parent,
         An8.V100.mesh m,
         Object tag)
      {
         String name = MakeUnique(m.name?.text);
         point origin = m.@base?.origin?.point ?? new point();
         quaternion orientation = m.@base?.orientation?.quaternion ??
            new quaternion(0, 0, 0, 1);
         Double[] matrix = orientation.ToMatrix(origin);
         VisualNode node = new VisualNode(name, matrix, tag);
         node.GeometryId = name;
         node.Parent = parent;
         parent.Children.Add(node);

         geometry geometry = new geometry();
         library.geometry = library.geometry.Append(geometry);

         geometry.id = node.Id;
         geometry.name = geometry.id;

         Dae.V141.mesh mesh = new Dae.V141.mesh();
         geometry.Item = mesh;

         // Add source for points
         String pointsSourceId = null;
         if ( m.points?.point != null )
         {
            source source = new source();
            mesh.source = mesh.source.Append(source);

            source.id = MakeUnique(geometry.id + "-positions");
            pointsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array");

            Double[] values = new Double[m.points.point.Length * 3];

            for ( Int64 i = 0; i < m.points.point.Length; i++ )
            {
               values[i * 3 + 0] = m.points.point[i].x;
               values[i * 3 + 1] = m.points.point[i].y;
               values[i * 3 + 2] = m.points.point[i].z;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)m.points.point.Length;
            accessor.stride = 3;

            // Add x param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "X";
               param.type = "float";
            }

            // Add y param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "Y";
               param.type = "float";
            }

            // Add z param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "Z";
               param.type = "float";
            }
         }

         // Calculate normals if they are missing
         An8Normals.Calculate(m);

         // Add source for normals
         String normalsSourceId = null;
         if ( m.normals?.point != null )
         {
            source source = new source();
            mesh.source = mesh.source.Append(source);

            source.id = MakeUnique(geometry.id + "-normals");
            normalsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array");

            Double[] values = new Double[m.normals.point.Length * 3];

            for ( Int64 i = 0; i < m.normals.point.Length; i++ )
            {
               values[i * 3 + 0] = m.normals.point[i].x;
               values[i * 3 + 1] = m.normals.point[i].y;
               values[i * 3 + 2] = m.normals.point[i].z;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)m.normals.point.Length;
            accessor.stride = 3;

            // Add x param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "X";
               param.type = "float";
            }

            // Add y param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "Y";
               param.type = "float";
            }

            // Add z param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "Z";
               param.type = "float";
            }
         }

         // Add source for texcoords
         String texcoordsSourceId = null;
         if ( m.texcoords?.texcoord != null )
         {
            source source = new source();
            mesh.source = mesh.source.Append(source);

            source.id = MakeUnique(geometry.id + "-map-0");
            texcoordsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array");

            Double[] values = new Double[m.texcoords.texcoord.Length * 2];

            for ( Int64 i = 0; i < m.texcoords.texcoord.Length; i++ )
            {
               values[i * 2 + 0] = m.texcoords.texcoord[i].u;
               values[i * 2 + 1] = m.texcoords.texcoord[i].v;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)m.texcoords.texcoord.Length;
            accessor.stride = 2;

            // Add s param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "S";
               param.type = "float";
            }

            // Add t param
            {
               param param = new param();
               accessor.param = accessor.param.Append(param);

               param.name = "T";
               param.type = "float";
            }
         }

         // Add vertices
         String verticesSourceId = null;
         if ( pointsSourceId != null )
         {
            vertices vertices = new vertices();
            mesh.vertices = vertices;

            vertices.id = MakeUnique(geometry.id + "-vertices");
            verticesSourceId = vertices.id;

            InputLocal input = new InputLocal();
            vertices.input = vertices.input.Append(input);

            input.semantic = "POSITION";
            input.source = "#" + pointsSourceId;
         }

         if ( m.faces?.facedata != null )
         {
            // Add polylist
            polylist polylist = new polylist();
            mesh.Items = mesh.Items.Append(polylist);

            // TODO: Set polylist.material

            polylist.count = (UInt64)m.faces.facedata.Length;

            UInt64 offset = 0;

            // Add vertex input
            if ( verticesSourceId != null )
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = polylist.input.Append(input);

               input.semantic = "VERTEX";
               input.source = "#" + verticesSourceId;
               input.offset = offset++;
            }

            // Add normal input
            if ( normalsSourceId != null )
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = polylist.input.Append(input);

               input.semantic = "NORMAL";
               input.source = "#" + normalsSourceId;
               input.offset = offset++;
            }

            // Add texcoord input
            if ( texcoordsSourceId != null )
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = polylist.input.Append(input);

               input.semantic = "TEXCOORD";
               input.source = "#" + texcoordsSourceId;
               input.offset = offset++;
               input.set = 0;
            }

            // Add face vertex counts
            {
               StringBuilder sb = new StringBuilder();

               for ( Int64 i = 0; i < m.faces.facedata.Length; i++ )
               {
                  // Note: We could use 'numpoints', but pointdata
                  // contains the actual list of points, so it is safer.
                  sb.Append(m.faces.facedata[i].pointdata.Length);

                  if ( i < m.faces.facedata.Length - 1 )
                  {
                     sb.Append(' ');
                  }
               }

               polylist.vcount = sb.ToString();
            }

            // Add face indices
            {
               StringBuilder sb = new StringBuilder();

               for ( Int64 i = 0; i < m.faces.facedata.Length; i++ )
               {
                  pointdata[] pointdata = m.faces.facedata[i].pointdata;

                  // Note: An8 specifies faces in clockwise order, while
                  // Dae specifies faces in counter-clockwise order.
                  for ( Int64 j = pointdata.Length - 1; j >= 0; j-- )
                  {
                     // Add point index if needed
                     if ( pointsSourceId != null )
                     {
                        sb.Append(pointdata[j].pointindex);
                     }

                     // Add normal index if needed
                     if ( normalsSourceId != null )
                     {
                        sb.Append(' ');

                        // Note: An8 supports enabling/disabling normals
                        // per face. Dae only supports enabling/disabling
                        // normals for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(pointdata[j].normalindex);
                     }

                     // Add texcoord index if needed
                     if ( texcoordsSourceId != null )
                     {
                        sb.Append(' ');

                        // Note: An8 supports enabling/disabling textures
                        // per face. Dae only supports enabling/disabling
                        // textures for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(pointdata[j].texcoordindex);
                     }

                     if ( i < m.faces.facedata.Length - 1 || j > 0 )
                     {
                        sb.Append(' ');
                     }
                  }
               }

               polylist.p = sb.ToString();
            }
         }

         return node;
      }

      static void AddBoneGeometry(
         ANIM8OR an8,
         library_geometries library,
         VisualNode parent,
         bone2 b,
         Object tag,
         point p = new point())
      {
         // Convert the bone to a group
         group2 g = new group2();
         g.name = new @string() { text = b.name };
         g.@base = new @base()
         {
            origin = new origin() { point = p },
            orientation = b.orientation,
         };
         g.mesh = b.mesh;
         g.sphere = b.sphere;
         g.cylinder = b.cylinder;
         g.cube = b.cube;
         g.subdivision = b.subdivision;
         g.pathcom = b.pathcom;
         g.textcom = b.textcom;
         g.modifier = b.modifier;
         g.image = b.image;
         g.group = b.group;

         VisualNode node = AddGroup(library, parent, g, tag);

         foreach ( namedobject n in b.namedobject ?? new namedobject[0] )
         {
            // Convert the namedobject to a group
            group2 g2 = new group2();
            g2.name = n.name;
            g2.@base = n.@base;

            VisualNode nNode = AddGroup(library, node, g2, n);

            // Scale the node
            Double scale = n.scale?.text ?? 1.0;

            if ( scale != 1.0 )
            {
               nNode.Matrix = nNode.Matrix ?? IDENTITY;
               nNode.Matrix[0] *= scale;
               nNode.Matrix[5] *= scale;
               nNode.Matrix[10] *= scale;
            }

            // Find the object that was named
            foreach ( @object o in an8?.@object ?? new @object[0] )
            {
               if ( o.name == n.objectname )
               {
                  // Find the original node of the object
                  VisualNode oNode = sVisualNode.Find(
                     (VisualNode v) => v.Tag == o);

                  if ( oNode != null )
                  {
                     CloneChildren(nNode, oNode);
                  }
                  break;
               }
            }
         }

         Double length = b.length?.text ?? 0.0;
         point p2 = new point(0, length, 0);

         foreach ( bone2 b2 in b.bone ?? new bone2[0] )
         {
            AddBoneGeometry(an8, library, node, b2, b2, p2);
         }
      }

      static void CloneChildren(VisualNode first, VisualNode second)
      {
         foreach ( VisualNode secondChildNode in second.Children )
         {
            VisualNode firstChildNode = new VisualNode(
               MakeUnique(secondChildNode.Id),
               secondChildNode.Matrix,
               secondChildNode.Tag);

            firstChildNode.GeometryId = secondChildNode.GeometryId;
            firstChildNode.ControllerId = secondChildNode.ControllerId;
            firstChildNode.RootJointId = secondChildNode.RootJointId;
            firstChildNode.Parent = first;
            first.Children.Add(firstChildNode);

            CloneChildren(firstChildNode, secondChildNode);
         }
      }
      #endregion

      #region library_animations
      static void CreateLibraryAnimations(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
      }
      #endregion

      #region library_controllers
      static void CreateLibraryControllers(COLLADA dae, ANIM8OR an8)
      {
         library_controllers library = new library_controllers();
         dae.Items = dae.Items.Append(library);

         foreach ( figure f in an8?.figure ?? new figure[0] )
         {
            if ( f.bone != null )
            {
               VisualNode node = sVisualNode.Find(
                  (VisualNode v) => v.Tag == f.bone);

               if ( node != null )
               {
                  AddBoneController(an8, library, node, node);
               }
            }
         }
      }

      static void AddBoneController(
         ANIM8OR an8,
         library_controllers library,
         VisualNode node,
         VisualNode rootNode)
      {
         if ( node.Tag is namedobject n )
         {
            // TODO: Calculate weights if they are missing
            An8Weights.Calculate(
               n,
               an8.@object,
               node.Parent.Tag as bone2,
               rootNode.Tag as bone2);

            foreach ( weights w in n.weights ?? new weights[0] )
            {
               VisualNode mNode = node.Find(
                  (VisualNode v) => v.Tag is An8.V100.mesh m &&
                  m.name?.text == w.meshname);

               if ( mNode != null )
               {
                  Dae.V141.controller controller = new Dae.V141.controller();
                  library.controller = library.controller.Append(controller);

                  controller.id = MakeUnique(mNode.Id + "-skin");
                  controller.name = controller.id;

                  skin skin = new skin();
                  controller.Item = skin;

                  skin.source1 = "#" + mNode.GeometryId;

                  {
                     // TODO: What is the correct matrix?
                     Double[] bindMatrix = mNode.Matrix;

                     skin.bind_shape_matrix =
                        COLLADA.ConvertFromArray(bindMatrix);

                     // Convert the geometry node to a controller node
                     mNode.GeometryId = null;
                     mNode.ControllerId = controller.id;
                     mNode.RootJointId = rootNode.Id;
                  }

                  List<VisualNode> boneNodes = new List<VisualNode>();
                  FindAllBoneNodes(rootNode, boneNodes);

                  // TODO: This hides a warning in Autodesk, but doesn't change
                  // the displayed result.
                  boneNodes.Add(node);

                  // Arrange the bone nodes in the "weighedby" order
                  if ( n.weightedby != null )
                  {
                     for ( Int32 i = 0; i < n.weightedby.Length; i++ )
                     {
                        String boneName = n.weightedby[i].text;

                        for ( Int32 j = 0; j < boneNodes.Count; j++ )
                        {
                           if ( boneNodes[j].Tag is bone2 b &&
                                b.name == boneName )
                           {
                              VisualNode temp = boneNodes[i];
                              boneNodes[i] = boneNodes[j];
                              boneNodes[j] = temp;
                              break;
                           }
                        }
                     }
                  }

                  // Add source for joints
                  String jointsSourceId = null;
                  {
                     source source = new source();
                     skin.source = skin.source.Append(source);

                     source.id = MakeUnique(controller.id + "-joints");
                     jointsSourceId = source.id;

                     Name_array array = new Name_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array");

                     String[] values = new String[boneNodes.Count];

                     for ( Int64 i = 0; i < boneNodes.Count; i++ )
                     {
                        values[i] = boneNodes[(Int32)i].Id;
                     }

                     array.count = (UInt64)values.Length;
                     array.Values = values;

                     source.technique_common = new sourceTechnique_common();

                     accessor accessor = new accessor();
                     source.technique_common.accessor = accessor;

                     accessor.source = "#" + array.id;
                     accessor.count = (UInt64)values.Length;
                     accessor.stride = 1;

                     // Add joint param
                     {
                        param param = new param();
                        accessor.param = accessor.param.Append(param);

                        param.name = "JOINT";
                        param.type = "name";
                     }
                  }

                  // Add source for bind poses
                  String bindPosesSourceId = null;
                  {
                     source source = new source();
                     skin.source = skin.source.Append(source);

                     source.id = MakeUnique(controller.id + "-bind_poses");
                     bindPosesSourceId = source.id;

                     float_array array = new float_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array");

                     Double[] values = new Double[boneNodes.Count * 16];

                     for ( Int64 i = 0; i < boneNodes.Count; i++ )
                     {
                        Double[] matrix = CalculateInverseBind(
                           boneNodes[(Int32)i]);

                        for ( Int64 j = 0; j < matrix.Length; j++ )
                        {
                           values[i * 16 + j] = matrix[j];
                        }
                     }

                     array.count = (UInt64)values.Length;
                     array.Values = values;

                     source.technique_common = new sourceTechnique_common();

                     accessor accessor = new accessor();
                     source.technique_common.accessor = accessor;

                     accessor.source = "#" + array.id;
                     accessor.count = (UInt64)boneNodes.Count;
                     accessor.stride = 16;

                     // Add transform param
                     {
                        param param = new param();
                        accessor.param = accessor.param.Append(param);

                        param.name = "TRANSFORM";
                        param.type = "float4x4";
                     }
                  }

                  // Add source for weights
                  String weightsSourceId = null;
                  {
                     source source = new source();
                     skin.source = skin.source.Append(source);

                     source.id = MakeUnique(controller.id + "-weights");
                     weightsSourceId = source.id;

                     float_array array = new float_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array");

                     List<Double> values = new List<Double>();

                     foreach ( weightdata w2 in w.weightdata )
                     {
                        foreach ( bonedata b in w2.bonedata )
                        {
                           values.Add(b.boneweight);
                        }
                     }

                     array.count = (UInt64)values.Count;
                     array.Values = values.ToArray();

                     source.technique_common = new sourceTechnique_common();

                     accessor accessor = new accessor();
                     source.technique_common.accessor = accessor;

                     accessor.source = "#" + array.id;
                     accessor.count = (UInt64)values.Count;
                     accessor.stride = 1;

                     // Add weight param
                     {
                        param param = new param();
                        accessor.param = accessor.param.Append(param);

                        param.name = "WEIGHT";
                        param.type = "float";
                     }
                  }

                  // Add joints
                  {
                     skinJoints joints = new skinJoints();
                     skin.joints = joints;

                     // Add joint input
                     if ( jointsSourceId != null )
                     {
                        InputLocal input = new InputLocal();
                        joints.input = joints.input.Append(input);

                        input.semantic = "JOINT";
                        input.source = "#" + jointsSourceId;
                     }

                     // Add bind pose input
                     if ( bindPosesSourceId != null )
                     {
                        InputLocal input = new InputLocal();
                        joints.input = joints.input.Append(input);

                        input.semantic = "INV_BIND_MATRIX";
                        input.source = "#" + bindPosesSourceId;
                     }
                  }

                  {
                     // Add weights
                     skinVertex_weights weights = new skinVertex_weights();
                     skin.vertex_weights = weights;

                     weights.count = (UInt64)w.weightdata.Length;

                     UInt64 offset = 0;

                     // Add joint input
                     if ( jointsSourceId != null )
                     {
                        InputLocalOffset input = new InputLocalOffset();
                        weights.input = weights.input.Append(input);

                        input.semantic = "JOINT";
                        input.source = "#" + jointsSourceId;
                        input.offset = offset++;
                     }

                     // Add weight input
                     if ( weightsSourceId != null )
                     {
                        InputLocalOffset input = new InputLocalOffset();
                        weights.input = weights.input.Append(input);

                        input.semantic = "WEIGHT";
                        input.source = "#" + weightsSourceId;
                        input.offset = offset++;
                     }

                     // Add bone weight counts
                     {
                        StringBuilder sb = new StringBuilder();

                        for ( Int64 i = 0; i < w.weightdata.Length; i++ )
                        {
                           // Note: We could use 'numweights', but bonedata
                           // contains the actual list of weights, so it is
                           // safer.
                           sb.Append(w.weightdata[i].bonedata.Length);

                           if ( i < w.weightdata.Length - 1 )
                           {
                              sb.Append(' ');
                           }
                        }

                        weights.vcount = sb.ToString();
                     }

                     // Add indices
                     {
                        StringBuilder sb = new StringBuilder();
                        Int64 index = 0;

                        for ( Int64 i = 0; i < w.weightdata.Length; i++ )
                        {
                           bonedata[] bonedata = w.weightdata[i].bonedata;

                           for ( Int64 j = 0; j < bonedata.Length; j++ )
                           {
                              sb.Append(bonedata[j].boneindex);
                              sb.Append(' ');
                              sb.Append(index++);

                              if ( i < w.weightdata.Length - 1 ||
                                   j < bonedata.Length - 1 )
                              {
                                 sb.Append(' ');
                              }
                           }
                        }

                        weights.v = sb.ToString();
                     }
                  }
               }
            }
         }

         foreach ( VisualNode child in node.Children )
         {
            AddBoneController(an8, library, child, rootNode);
         }
      }

      static void FindAllBoneNodes(VisualNode node, List<VisualNode> boneNodes)
      {
         if ( node.Tag is bone2 b )
         {
            boneNodes.Add(node);
         }

         foreach ( VisualNode child in node.Children )
         {
            FindAllBoneNodes(child, boneNodes);
         }
      }

      static readonly Double[] IDENTITY = new Double[]
      {
         1, 0, 0, 0,
         0, 1, 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1,
      };

      #region Inverse
      static Double[] Inverse(Double[] matrix)
      {
         if ( matrix != null )
         {
            Double invf(Int64 i, Int64 j, Double[] m)
            {
               Int64 o = 2 + (j - i);

               i += 4 + o;
               j += 4 - o;

               Double e(Int64 a, Int64 b)
               {
                  return m[(j + b) % 4 * 4 + (i + a) % 4];
               }

               Double inv =
                + e(+1, -1) * e(+0, +0) * e(-1, +1)
                + e(+1, +1) * e(+0, -1) * e(-1, +0)
                + e(-1, -1) * e(+1, +0) * e(+0, +1)
                - e(-1, -1) * e(+0, +0) * e(+1, +1)
                - e(-1, +1) * e(+0, -1) * e(+1, +0)
                - e(+1, -1) * e(-1, +0) * e(+0, +1);

               return o % 2 != 0 ? inv : -inv;
            }

            Double[] inverse = new Double[16];

            for ( Int64 i = 0; i < 4; i++ )
            {
               for ( Int64 j = 0; j < 4; j++ )
               {
                  inverse[j * 4 + i] = invf(i, j, matrix);
               }
            }

            Double determinant = 0;

            for ( Int64 k = 0; k < 4; k++ )
            {
               determinant += matrix[k] * inverse[k * 4];
            }

            if ( determinant != 0 )
            {
               determinant = 1 / determinant;

               for ( Int64 i = 0; i < 16; i++ )
               {
                  inverse[i] = inverse[i] * determinant;
               }

               return inverse;
            }
         }

         return null;
      }
      #endregion

      #region Multiply
      static Double[] Multiply(Double[] a, Double[] b)
      {
         if ( a != null && b != null )
         {
            Double[] matrix = new Double[16];

            for ( Int32 i = 0; i < 4; i++ )
            {
               for ( Int32 j = 0; j < 4; j++ )
               {
                  Double e = 0;

                  for ( Int32 k = 0; k < 4; k++ )
                  {
                     e += a[i + k * 4] * b[k + j * 4];
                  }

                  matrix[i + j * 4] = e;
               }
            }

            return matrix;
         }
         else if ( a != null )
         {
            return a;
         }
         else
         {
            return b;
         }
      }
      #endregion

      static Double[] CalculateInverseBind(VisualNode node)
      {
         Double[] matrix = node.Matrix;

         VisualNode iterator = node.Parent;

         while ( iterator != null )
         {
            matrix = Multiply(matrix, iterator.Matrix);

            iterator = iterator.Parent;
         }

         return Inverse(matrix) ?? IDENTITY;
      }
      #endregion

      #region library_visual_scenes
      class VisualNode
      {
         public VisualNode(
            String id = null,
            Double[] matrix = null,
            Object tag = null)
         {
            Id = id;
            Matrix = matrix;
            Tag = tag;
            Children = new List<VisualNode>();
         }

         public VisualNode Find(Condition condition)
         {
            if ( condition(this) )
            {
               return this;
            }
            else
            {
               VisualNode match = null;

               foreach ( VisualNode child in Children )
               {
                  match = child.Find(condition);

                  if ( match != null )
                  {
                     break;
                  }
               }

               return match;
            }
         }

         public delegate Boolean Condition(VisualNode node);
         public String Id;
         public Double[] Matrix;
         public String GeometryId;
         public String ControllerId;
         public String RootJointId;
         public Object Tag;
         public VisualNode Parent;
         public List<VisualNode> Children;
      }

      static List<String> sNames = new List<String>();
      static VisualNode sVisualNode = new VisualNode();

      static void CreateLibraryVisualScenes(COLLADA dae, ANIM8OR an8)
      {
         library_visual_scenes library = new library_visual_scenes();
         dae.Items = dae.Items.Append(library);

         visual_scene scene = new visual_scene();
         library.visual_scene = library.visual_scene.Append(scene);

         scene.id = sVisualNode.Id;
         scene.name = scene.id;

         void AddNode(VisualNode visualNode, node parentNode = null)
         {
            node node = new node();

            if ( parentNode == null )
            {
               scene.node = scene.node.Append(node);
            }
            else
            {
               parentNode.node1 = parentNode.node1.Append(node);
            }

            node.id = visualNode.Id;
            node.name = node.id;
            node.sid = node.id;

            if ( visualNode.Tag is bone2 )
            {
               node.type = NodeType.JOINT;
            }
            else
            {
               node.type = NodeType.NODE;
            }

            if ( visualNode.Matrix != null )
            {
               matrix m = new matrix();
               node.Items = node.Items.Append(m);

               m.Values = visualNode.Matrix;

               node.ItemsElementName = new ItemsChoiceType7[]
               {
                  ItemsChoiceType7.matrix,
               };
            }

            if ( visualNode.GeometryId != null )
            {
               instance_geometry i = new instance_geometry();
               node.instance_geometry = node.instance_geometry.Append(i);

               i.url = "#" + visualNode.GeometryId;
            }

            if ( visualNode.ControllerId != null )
            {
               instance_controller i = new instance_controller();
               node.instance_controller = node.instance_controller.Append(i);

               i.url = "#" + visualNode.ControllerId;
               i.skeleton = new String[] { "#" + visualNode.RootJointId };
            }

            foreach ( VisualNode childNode in visualNode.Children )
            {
               AddNode(childNode, node);
            }
         }

         foreach ( VisualNode visualNode in sVisualNode.Children )
         {
            AddNode(visualNode);
         }
      }
      #endregion

      #region scene
      static void CreateScene(COLLADA dae, ANIM8OR an8)
      {
         dae.scene = new COLLADAScene();
         dae.scene.instance_visual_scene = new InstanceWithExtra();
         dae.scene.instance_visual_scene.url = "#" + sVisualNode.Id;
      }
      #endregion
   }
}
