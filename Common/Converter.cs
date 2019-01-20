using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using Anim8orTransl8or.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Anim8orTransl8or
{
   public static class Converter
   {
      public static IEnumerable<Result> Convert(ANIM8OR an8)
      {
         // Create each object as a separate dae file
         foreach ( @object @object in an8?.@object ?? new @object[0] )
         {
            COLLADA dae = new COLLADA();
            List<String> usedNames = new List<String>();

            CreateAsset(an8, dae);

            VisualNode node = CreateLibraryGeometries(
               an8,
               dae,
               usedNames,
               (@object o) => @object == o,
               (figure f) => false);

            // The 'node' contains the object node. To prevent unnecessary
            // nesting, just use the object node instead.
            if ( node.Children.Count > 0 )
            {
               VisualNode oNode = node.Children[0];
               CreateLibraryVisualScenes(an8, dae, oNode);
               CreateScene(dae, an8, oNode);

               yield return new Result()
               {
                  Mode = Result.An8Mode.Object,
                  Name = @object.name ?? "Unnamed",
                  Dae = dae,
               };
            }
         }

         // Create each figure as a separate dae file
         foreach ( figure figure in an8?.figure ?? new figure[0] )
         {
            COLLADA dae = new COLLADA();
            List<String> usedNames = new List<String>();

            CreateAsset(an8, dae);

            VisualNode node = CreateLibraryGeometries(
               an8,
               dae,
               usedNames,
               (@object o) => false,
               (figure f) => figure == f);

            CreateLibraryControllers(an8, dae, usedNames, node);

            // The 'node' contains the figure node. To prevent unnecessary
            // nesting, just use the figure node instead.
            if ( node.Children.Count > 0 )
            {
               VisualNode fNode = node.Children[0];
               CreateLibraryVisualScenes(an8, dae, fNode);
               CreateScene(dae, an8, fNode);

               yield return new Result()
               {
                  Mode = Result.An8Mode.Figure,
                  Name = figure.name ?? "Unnamed",
                  Dae = dae,
               };
            }
         }

         // Create each sequence as a separate dae file
         foreach ( sequence sequence in an8?.sequence ?? new sequence[0] )
         {
            COLLADA dae = new COLLADA();
            List<String> usedNames = new List<String>();

            CreateAsset(an8, dae);

            VisualNode node = CreateLibraryGeometries(
               an8,
               dae,
               usedNames,
               (@object o) => false,
               (figure f) => sequence.figure?.text == f.name);

            CreateLibraryControllers(an8, dae, usedNames, node);
            CreateLibraryAnimations(an8, dae, usedNames, node);

            // The 'node' contains the figure node. To prevent unnecessary
            // nesting, just use the figure node instead.
            if ( node.Children.Count > 0 )
            {
               VisualNode fNode = node.Children[0];
               CreateLibraryVisualScenes(an8, dae, fNode);
               CreateScene(dae, an8, fNode);

               yield return new Result()
               {
                  Mode = Result.An8Mode.Sequence,
                  Name = sequence.name ?? "Unnamed",
                  Dae = dae,
               };
            }
         }
      }

      #region asset
      static void CreateAsset(ANIM8OR an8, COLLADA dae)
      {
         AssemblyName assembly = typeof(Converter).Assembly.GetName();

         dae.asset = new asset();
         dae.asset.contributor = new assetContributor[1];
         dae.asset.contributor[0] = new assetContributor();
         dae.asset.contributor[0].author = "Anim8or v" +
            an8.header?.version?.text ?? "0.0.0" + " build " +
            an8.header?.build?.text ?? "1970.1.1";
         dae.asset.contributor[0].authoring_tool =
            assembly.Name + " v" + assembly.Version.ToString(3);
         dae.asset.contributor[0].comments =
            String.Join("\r\n", an8.description?.text ?? new String[0]);
         dae.asset.created = DateTime.Now;
         dae.asset.modified = dae.asset.created;
         dae.asset.unit = new assetUnit();
         dae.asset.unit.name = "meter";
         dae.asset.unit.meter = 1;
         dae.asset.up_axis = UpAxisType.Y_UP;
      }
      #endregion

      #region library_geometries
      static VisualNode CreateLibraryGeometries(
         ANIM8OR an8,
         COLLADA dae,
         List<String> usedNames,
         Func<@object, Boolean> includeObject = null,
         Func<figure, Boolean> includeFigure = null)
      {
         VisualNode node = new VisualNode(null, An8.matrix.IDENTITY, null);

         library_geometries library = new library_geometries();
         dae.Items = dae.Items.Append(library);

         foreach ( @object @object in an8.@object ?? new @object[0] )
         {
            // Use a callback to filter out unwanted objects
            if ( includeObject != null && !includeObject(@object) )
            {
               continue;
            }

            // Convert the object to a group
            group1 group = new group1();
            group.name = new @string() { text = @object?.name };
            group.mesh = @object?.mesh;
            group.sphere = @object?.sphere;
            group.cylinder = @object?.cylinder;
            group.cube = @object?.cube;
            group.subdivision = @object?.subdivision;
            group.pathcom = @object?.pathcom;
            group.textcom = @object?.textcom;
            group.modifier = @object?.modifier;
            group.image = @object?.image;
            group.group = @object?.group;

            VisualNode oNode = AddGroup(usedNames, library, group, @object);
            node.Link(oNode);
         }

         foreach ( figure figure in an8.figure ?? new figure[0] )
         {
            // Use a callback to filter out unwanted figures
            if ( includeFigure != null && !includeFigure(figure) )
            {
               continue;
            }

            // Convert the figure to a group
            group1 group = new group1();
            group.name = new @string() { text = figure?.name };

            VisualNode fNode = AddGroup(usedNames, library, group, figure);

            if ( figure?.bone != null )
            {
               VisualNode bNode = AddBoneGeometry(
                  an8,
                  usedNames,
                  library,
                  figure.bone,
                  new point(),
                  true);

               fNode.Link(bNode);
            }

            node.Link(fNode);
         }

         return node;
      }

      static VisualNode AddGroup(
         List<String> usedNames,
         library_geometries library,
         group1 group,
         Object tag,
         Double scale = 1)
      {
         String name = MakeUnique(group.name?.text, usedNames);
         point origin = group.@base?.origin?.point ?? new point();
         quaternion orientation = group.@base?.orientation?.quaternion ??
            quaternion.IDENTITY;
         An8.matrix matrix = new An8.matrix(origin, orientation, scale);
         VisualNode node = new VisualNode(name, matrix, tag);

         foreach ( An8.V100.mesh mesh in group.mesh ?? new An8.V100.mesh[0] )
         {
            VisualNode mNode = AddMesh(usedNames, library, mesh, mesh);
            node.Link(mNode);
         }

         foreach ( An8.V100.sphere sphere in group.sphere ??
            new An8.V100.sphere[0] )
         {
            An8.V100.mesh mesh = An8Sphere.Calculate(sphere);
            VisualNode sNode = AddMesh(usedNames, library, mesh, sphere);
            node.Link(sNode);
         }

         foreach ( An8.V100.cylinder cylinder in group.cylinder ??
            new An8.V100.cylinder[0] )
         {
            An8.V100.mesh mesh = An8Cylinder.Calculate(cylinder);
            VisualNode cNode = AddMesh(usedNames, library, mesh, cylinder);
            node.Link(cNode);
         }

         foreach ( cube cube in group.cube ?? new cube[0] )
         {
            An8.V100.mesh mesh = An8Cube.Calculate(cube);
            VisualNode cNode = AddMesh(usedNames, library, mesh, cube);
            node.Link(cNode);
         }

         foreach ( subdivision subdivision in group.subdivision ??
            new subdivision[0] )
         {
            An8.V100.mesh mesh = An8Subdivision.Calculate(subdivision);
            VisualNode sNode = AddMesh(usedNames, library, mesh, subdivision);
            node.Link(sNode);
         }

         foreach ( pathcom pathcom in group.pathcom ?? new pathcom[0] )
         {
            An8.V100.mesh mesh = An8PathCom.Calculate(pathcom);
            VisualNode pNode = AddMesh(usedNames, library, mesh, pathcom);
            node.Link(pNode);
         }

         foreach ( textcom textcom in group.textcom ?? new textcom[0] )
         {
            An8.V100.mesh mesh = An8TextCom.Calculate(textcom);
            VisualNode tNode = AddMesh(usedNames, library, mesh, textcom);
            node.Link(tNode);
         }

         foreach ( modifier1 modifier in group.modifier ?? new modifier1[0] )
         {
            // Convert the modifier to a group
            // Note: The modifier's base/pivot only affects the modifier.
            group1 group2 = new group1();
            group2.name = modifier?.name;
            group2.mesh = group2.mesh.Append(modifier?.mesh);
            group2.sphere= group2.sphere.Append(modifier?.sphere);
            group2.cylinder = group2.cylinder.Append(modifier?.cylinder);
            group2.cube= group2.cube.Append(modifier?.cube);
            group2.subdivision = group2.subdivision.Append(
               modifier?.subdivision);
            group2.pathcom = group2.pathcom.Append(modifier?.pathcom);
            group2.textcom = group2.textcom.Append(modifier?.textcom);
            group2.modifier = group2.modifier.Append(modifier?.modifier);
            group2.image = group2.image.Append(modifier?.image);
            group2.group = group2.group.Append(modifier?.group);

            VisualNode mNode = AddGroup(usedNames, library, group2, modifier);
            node.Link(mNode);
         }

         foreach ( An8.V100.image image in group.image ??
            new An8.V100.image[0] )
         {
            An8.V100.mesh mesh = An8Image.Calculate(image);
            VisualNode iNode = AddMesh(usedNames, library, mesh, image);
            node.Link(iNode);
         }

         foreach ( group1 group2 in group.group ?? new group1[0] )
         {
            VisualNode gNode = AddGroup(usedNames, library, group2, group2);
            node.Link(gNode);
         }

         return node;
      }

      static VisualNode AddMesh(
         List<String> usedNames,
         library_geometries library,
         An8.V100.mesh mesh,
         Object tag,
         Double scale = 1)
      {
         // Calculate normals if they are missing
         if ( mesh.normals?.point == null )
         {
            mesh = An8Normals.Calculate(mesh);
         }

         String name = MakeUnique(mesh.name?.text, usedNames);
         point origin = mesh.@base?.origin?.point ?? new point();
         quaternion orientation = mesh.@base?.orientation?.quaternion ??
            quaternion.IDENTITY;
         An8.matrix matrix = new An8.matrix(origin, orientation, scale);
         VisualNode node = new VisualNode(name, matrix, tag);
         node.Mesh = mesh;
         node.GeometryId = name;

         geometry geometry = new geometry();
         library.geometry = library.geometry.Append(geometry);

         geometry.id = name;
         geometry.name = name;

         Dae.V141.mesh mesh2 = new Dae.V141.mesh();
         geometry.Item = mesh2;

         // Add source for points
         String pointsSourceId = null;
         if ( mesh.points?.point != null )
         {
            source source = new source();
            mesh2.source = mesh2.source.Append(source);

            source.id = MakeUnique(geometry.id + "-positions", usedNames);
            pointsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array", usedNames);

            Double[] values = new Double[mesh.points.point.Length * 3];
            Int32 index = 0;

            foreach ( point point in mesh.points.point )
            {
               values[index++] = point.x;
               values[index++] = point.y;
               values[index++] = point.z;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)values.Length / 3;
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

         // Add source for normals
         String normalsSourceId = null;
         if ( mesh.normals?.point != null )
         {
            source source = new source();
            mesh2.source = mesh2.source.Append(source);

            source.id = MakeUnique(geometry.id + "-normals", usedNames);
            normalsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array", usedNames);

            Double[] values = new Double[mesh.normals.point.Length * 3];
            Int32 index = 0;

            foreach ( point normal in mesh.normals.point )
            {
               values[index++] = normal.x;
               values[index++] = normal.y;
               values[index++] = normal.z;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)values.Length / 3;
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
         if ( mesh.texcoords?.texcoord != null )
         {
            source source = new source();
            mesh2.source = mesh2.source.Append(source);

            source.id = MakeUnique(geometry.id + "-map-0", usedNames);
            texcoordsSourceId = source.id;

            float_array array = new float_array();
            source.Item = array;

            array.id = MakeUnique(source.id + "-array", usedNames);

            Double[] values = new Double[mesh.texcoords.texcoord.Length * 2];
            Int32 index = 0;

            foreach ( texcoord texcoord in mesh.texcoords.texcoord )
            {
               values[index++] = texcoord.u;
               values[index++] = texcoord.v;
            }

            array.count = (UInt64)values.Length;
            array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + array.id;
            accessor.count = (UInt64)values.Length / 2;
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
            mesh2.vertices = vertices;

            vertices.id = MakeUnique(geometry.id + "-vertices", usedNames);
            verticesSourceId = vertices.id;

            InputLocal input = new InputLocal();
            vertices.input = vertices.input.Append(input);

            input.semantic = "POSITION";
            input.source = "#" + pointsSourceId;
         }

         // Add faces
         if ( mesh.faces?.facedata != null )
         {
            polylist polylist = new polylist();
            mesh2.Items = mesh2.Items.Append(polylist);

            // TODO: Set polylist.material

            polylist.count = (UInt64)mesh.faces.facedata.Length;

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

               foreach ( facedata facedata in mesh.faces.facedata )
               {
                  // Note: We could use 'numpoints', but pointdata
                  // contains the actual list of points, so it is safer.
                  sb.Append(facedata.pointdata?.Length ?? 0);
                  sb.Append(' ');
               }

               polylist.vcount = sb.ToString().TrimEnd();
            }

            // Add face indices
            {
               StringBuilder sb = new StringBuilder();

               foreach ( facedata facedata in mesh.faces.facedata )
               {
                  pointdata[] pointdata = facedata.pointdata;

                  // Note: An8 specifies faces in clockwise order, while
                  // Dae specifies faces in counter-clockwise order.
                  for ( Int32 j = (pointdata?.Length ?? 0) - 1; j >= 0; j-- )
                  {
                     // Add point index if needed
                     if ( pointsSourceId != null )
                     {
                        sb.Append(pointdata[j].pointindex);
                        sb.Append(' ');
                     }

                     // Add normal index if needed
                     if ( normalsSourceId != null )
                     {
                        // Note: An8 supports enabling/disabling normals
                        // per face. Dae only supports enabling/disabling
                        // normals for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(pointdata[j].normalindex);
                        sb.Append(' ');
                     }

                     // Add texcoord index if needed
                     if ( texcoordsSourceId != null )
                     {
                        // Note: An8 supports enabling/disabling textures
                        // per face. Dae only supports enabling/disabling
                        // textures for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(pointdata[j].texcoordindex);
                        sb.Append(' ');
                     }
                  }
               }

               polylist.p = sb.ToString().TrimEnd();
            }
         }

         return node;
      }

      static VisualNode AddBoneGeometry(
         ANIM8OR an8,
         List<String> usedNames,
         library_geometries library,
         bone1 bone,
         point origin,
         Boolean rootBone = false,
         Double scale = 1)
      {
         // Convert the bone to a group
         group1 group = new group1();
         group.name = new @string() { text = bone.name };
         group.@base = new @base()
         {
            origin = new origin() { point = origin },
            orientation = bone.orientation,
         };
         group.mesh = bone.mesh;
         group.sphere = bone.sphere;
         group.cylinder = bone.cylinder;
         group.cube = bone.cube;
         group.subdivision = bone.subdivision;
         group.pathcom = bone.pathcom;
         group.textcom = bone.textcom;
         group.modifier = bone.modifier;
         group.image = bone.image;
         group.group = bone.group;

         VisualNode node = AddGroup(usedNames, library, group, bone, scale);

         foreach ( namedobject namedobject in bone.namedobject ??
            new namedobject[0] )
         {
            // Convert the namedobject to a group
            group1 group2 = new group1();
            group2.name = namedobject?.name;
            group2.@base = namedobject?.@base;

            VisualNode nNode = AddGroup(
               usedNames,
               library,
               group2,
               namedobject,
               namedobject?.scale?.text ?? 1.0);

            node.Link(nNode);

            // Find the object that was named
            foreach ( @object @object in an8.@object ?? new @object[0] )
            {
               if ( @object?.name == namedobject?.objectname )
               {
                  // Convert the object to a group
                  group1 group3 = new group1();
                  group3.name = new @string() { text = @object?.name };
                  group3.mesh = @object?.mesh;
                  group3.sphere = @object?.sphere;
                  group3.cylinder = @object?.cylinder;
                  group3.cube = @object?.cube;
                  group3.subdivision = @object?.subdivision;
                  group3.pathcom = @object?.pathcom;
                  group3.textcom = @object?.textcom;
                  group3.modifier = @object?.modifier;
                  group3.image = @object?.image;
                  group3.group = @object?.group;

                  VisualNode oNode = AddGroup(
                     usedNames,
                     library,
                     group3,
                     @object);

                  nNode.Link(oNode);
                  break;
               }
            }
         }

         // Note: Anim8or v1.00 ignores the root bone's length.
         if ( !rootBone )
         {
            Double length = bone.length?.text ?? 0.0;
            origin = new point(0, length, 0);
         }

         foreach ( bone1 bone2 in bone.bone ?? new bone1[0] )
         {
            VisualNode bNode = AddBoneGeometry(
               an8,
               usedNames,
               library,
               bone2,
               origin);

            node.Link(bNode);
         }

         return node;
      }
      #endregion

      #region library_controllers
      static void CreateLibraryControllers(
         ANIM8OR an8,
         COLLADA dae,
         List<String> usedNames,
         VisualNode parentNode)
      {
         library_controllers library = new library_controllers();
         dae.Items = dae.Items.Append(library);

         foreach ( figure figure in an8.figure ?? new figure[0] )
         {
            if ( figure.bone != null )
            {
               VisualNode fNode = parentNode.Find(
                  v => v.Tag == figure);

               VisualNode bNode = fNode?.Children.Find(
                  v => v.Tag == figure.bone);

               if ( bNode != null )
               {
                  AddBoneController(
                     an8,
                     usedNames,
                     library,
                     bNode,
                     bNode);
               }
            }
         }
      }

      static void AddBoneController(
         ANIM8OR an8,
         List<String> usedNames,
         library_controllers library,
         VisualNode sNode,
         VisualNode node)
      {
         if ( node.Tag is namedobject namedobject )
         {
            // Calculate weights if they are missing
            if ( namedobject?.weights == null )
            {
               // Find the named object
               VisualNode oNode = sNode.Find(
                  v => v.Tag is @object o && o.name == namedobject.objectname);

               List<An8.V100.mesh> ms = new List<An8.V100.mesh>();

               foreach ( VisualNode vn in
                  oNode?.FindAll(v => v.Mesh != null) ?? new VisualNode[0] )
               {
                  ms.Add(vn.Mesh);
               }

               namedobject = An8Weights.Calculate(
                  namedobject,
                  ms.ToArray(),
                  sNode.Tag as bone1);
            }

            foreach ( weights weights in namedobject.weights ??
               new weights[0] )
            {
               VisualNode mNode = node.Find(
                  v => v.Mesh?.name?.text == weights.meshname);

               if ( mNode != null )
               {
                  Dae.V141.controller controller = new Dae.V141.controller();
                  library.controller = library.controller.Append(controller);

                  controller.id = MakeUnique(mNode.Id + "-skin", usedNames);
                  controller.name = controller.id;

                  skin skin = new skin();
                  controller.Item = skin;

                  skin.source1 = "#" + mNode.GeometryId;

                  Double[] bindMatrix =
                     mNode.BindMatrix().GetEnumerator().ToArray();

                  skin.bind_shape_matrix =
                     COLLADA.ConvertFromArray(bindMatrix);

                  // Convert the geometry node to a controller node
                  mNode.GeometryId = null;
                  mNode.ControllerId = controller.id;
                  mNode.SkeletonId = sNode.Id;

                  List<VisualNode> boneNodes = sNode.FindAll(
                     v => v.Tag is bone1).ToList();

                  // TODO: These prevent warnings in Autodesk, but don't change
                  // the displayed result. Are they needed?
                  {
                     VisualNode namedObjectNode = node;
                     VisualNode objectNode = node.Children.Find(
                        v => v.Tag is @object);

                     if ( namedObjectNode != null )
                     {
                        boneNodes.Add(namedObjectNode);
                     }

                     if ( objectNode != null )
                     {
                        boneNodes.Add(objectNode);
                     }
                  }

                  // Arrange the bone nodes in the "weighedby" order
                  for ( Int32 i = 0; i < namedobject.weightedby?.Length; i++ )
                  {
                     String boneName = namedobject.weightedby[i].text;

                     for ( Int32 j = 0; j < boneNodes.Count; j++ )
                     {
                        if ( (boneNodes[j].Tag as bone1)?.name == boneName )
                        {
                           VisualNode temp = boneNodes[i];
                           boneNodes[i] = boneNodes[j];
                           boneNodes[j] = temp;
                           break;
                        }
                     }
                  }

                  // Add source for joints
                  String jointsSourceId = null;
                  {
                     source source = new source();
                     skin.source = skin.source.Append(source);

                     source.id = MakeUnique(
                        controller.id + "-joints",
                        usedNames);

                     jointsSourceId = source.id;

                     Name_array array = new Name_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array", usedNames);

                     String[] values = new String[boneNodes.Count];

                     for ( Int32 i = 0; i < boneNodes.Count; i++ )
                     {
                        values[i] = boneNodes[i].Id;
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

                     source.id = MakeUnique(
                        controller.id + "-bind_poses",
                        usedNames);

                     bindPosesSourceId = source.id;

                     float_array array = new float_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array", usedNames);

                     Double[] values = new Double[boneNodes.Count * 16];
                     Int32 index = 0;

                     foreach ( VisualNode boneNode in boneNodes )
                     {
                        An8.matrix matrix = boneNode.BindMatrix().Inverse();

                        foreach ( Double value in matrix.GetEnumerator() )
                        {
                           values[index++] = value;
                        }
                     }

                     array.count = (UInt64)values.Length;
                     array.Values = values;

                     source.technique_common = new sourceTechnique_common();

                     accessor accessor = new accessor();
                     source.technique_common.accessor = accessor;

                     accessor.source = "#" + array.id;
                     accessor.count = (UInt64)values.Length / 16;
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

                     source.id = MakeUnique(
                        controller.id + "-weights",
                        usedNames);

                     weightsSourceId = source.id;

                     float_array array = new float_array();
                     source.Item = array;

                     array.id = MakeUnique(source.id + "-array", usedNames);

                     List<Double> values = new List<Double>();

                     foreach ( weightdata weightdata in weights.weightdata ??
                        new weightdata[0] )
                     {
                        foreach ( bonedata bonedata in weightdata.bonedata ??
                           new bonedata[0] )
                        {
                           values.Add(bonedata.boneweight);
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

                  // Add weights
                  {
                     skinVertex_weights weights2 = new skinVertex_weights();
                     skin.vertex_weights = weights2;

                     weights2.count = (UInt64)weights.weightdata?.Length;

                     UInt64 offset = 0;

                     // Add joint input
                     if ( jointsSourceId != null )
                     {
                        InputLocalOffset input = new InputLocalOffset();
                        weights2.input = weights2.input.Append(input);

                        input.semantic = "JOINT";
                        input.source = "#" + jointsSourceId;
                        input.offset = offset++;
                     }

                     // Add weight input
                     if ( weightsSourceId != null )
                     {
                        InputLocalOffset input = new InputLocalOffset();
                        weights2.input = weights2.input.Append(input);

                        input.semantic = "WEIGHT";
                        input.source = "#" + weightsSourceId;
                        input.offset = offset++;
                     }

                     // Add bone weight counts
                     {
                        StringBuilder sb = new StringBuilder();

                        foreach ( weightdata weightdata in
                           weights.weightdata ?? new weightdata[0] )
                        {
                           // Note: We could use 'numweights', but bonedata
                           // contains the actual list of weights, so it is
                           // safer.
                           sb.Append(weightdata.bonedata?.Length ?? 0);
                           sb.Append(' ');
                        }

                        weights2.vcount = sb.ToString().TrimEnd();
                     }

                     // Add indices
                     {
                        StringBuilder sb = new StringBuilder();
                        Int32 index = 0;

                        foreach ( weightdata weightdata in
                           weights.weightdata ?? new weightdata[0] )
                        {
                           foreach ( bonedata bonedata in
                              weightdata.bonedata ?? new bonedata[0] )
                           {
                              sb.Append(bonedata.boneindex);
                              sb.Append(' ');
                              sb.Append(index++);
                              sb.Append(' ');
                           }
                        }

                        weights2.v = sb.ToString().TrimEnd();
                     }
                  }
               }
            }
         }

         foreach ( VisualNode childNode in node.Children )
         {
            AddBoneController(
               an8,
               usedNames,
               library,
               sNode,
               childNode);
         }
      }
      #endregion

      #region library_animations
      static void CreateLibraryAnimations(
         ANIM8OR an8,
         COLLADA dae,
         List<String> usedNames,
         VisualNode parentNode)
      {
         library_animations library = new library_animations();
         dae.Items = dae.Items.Append(library);

         foreach ( sequence sequence in an8.sequence ?? new sequence[0] )
         {
            VisualNode fNode = parentNode.Find(
               v => v.Tag is figure f &&
               f.name == sequence.figure?.text);

            VisualNode bNode = fNode?.Children.Find(
               v => v.Tag == (fNode.Tag as figure)?.bone);

            if ( bNode != null )
            {
               List<jointangle> jointAngles = new List<jointangle>(
                  sequence.jointangle ?? new jointangle[0]);

               AddBoneAnimation(an8, usedNames, library, bNode, jointAngles);
            }
         }
      }

      static void AddBoneAnimation(
         ANIM8OR an8,
         List<String> usedNames,
         library_animations library,
         VisualNode node,
         List<jointangle> jointangles)
      {
         if ( node.Tag is bone1 bone )
         {
            SortedDictionary<Double, An8.matrix> frames =
               CalculateKeyFrames(
                  an8,
                  node,
                  jointangles.FindAll(j => j.bone == bone.name));

            jointangles.RemoveAll(j => j.bone == bone.name);

            animation animation = new animation();
            library.animation = library.animation.Append(animation);

            animation.id = MakeUnique(node.Id + "_pose_matrix", usedNames);

            // Add source for input
            String inputSourceId = null;
            if ( frames.Count > 0 )
            {
               source source = new source();
               animation.Items = animation.Items.Append(source);

               source.id = MakeUnique(animation.id + "-input", usedNames);
               inputSourceId = source.id;

               float_array array = new float_array();
               source.Item = array;

               array.id = MakeUnique(source.id + "-array", usedNames);
               Double[] values = new Double[frames.Count];
               Int32 index = 0;

               foreach ( KeyValuePair<Double, An8.matrix> frame in frames )
               {
                  values[index++] = frame.Key;
               }

               array.count = (UInt64)values.Length;
               array.Values = values;

               source.technique_common = new sourceTechnique_common();

               accessor accessor = new accessor();
               source.technique_common.accessor = accessor;

               accessor.source = "#" + array.id;
               accessor.count = (UInt64)values.Length;
               accessor.stride = 1;

               // Add time param
               {
                  param param = new param();
                  accessor.param = accessor.param.Append(param);

                  param.name = "TIME";
                  param.type = "float";
               }
            }

            // Add source for output
            String outputSourceId = null;
            if ( frames.Count > 0 )
            {
               source source = new source();
               animation.Items = animation.Items.Append(source);

               source.id = MakeUnique(animation.id + "-output", usedNames);
               outputSourceId = source.id;

               float_array array = new float_array();
               source.Item = array;

               array.id = MakeUnique(source.id + "-array", usedNames);
               Double[] values = new Double[frames.Count * 16];
               Int32 index = 0;

               foreach ( KeyValuePair<Double, An8.matrix> frame in frames )
               {
                  foreach ( Double value in frame.Value.GetEnumerator() )
                  {
                     values[index++] = value;
                  }
               }

               array.count = (UInt64)values.Length;
               array.Values = values;

               source.technique_common = new sourceTechnique_common();

               accessor accessor = new accessor();
               source.technique_common.accessor = accessor;

               accessor.source = "#" + array.id;
               accessor.count = (UInt64)values.Length / 16;
               accessor.stride = 16;

               // Add transform param
               {
                  param param = new param();
                  accessor.param = accessor.param.Append(param);

                  param.name = "TRANSFORM";
                  param.type = "float4x4";
               }
            }

            // Add source for interpolation
            String interpolationSourceId = null;
            if ( frames.Count > 0 )
            {
               source source = new source();
               animation.Items = animation.Items.Append(source);

               source.id = MakeUnique(animation.id + "-interpolation", usedNames);
               interpolationSourceId = source.id;

               Name_array array = new Name_array();
               source.Item = array;

               array.id = MakeUnique(source.id + "-array", usedNames);
               String[] values = new String[frames.Count];
               Int32 index = 0;

               foreach ( KeyValuePair<Double, An8.matrix> frame in frames )
               {
                  // TODO: Support more than just linear
                  values[index++] = "LINEAR";
               }

               array.count = (UInt64)values.Length;
               array.Values = values;

               source.technique_common = new sourceTechnique_common();

               accessor accessor = new accessor();
               source.technique_common.accessor = accessor;

               accessor.source = "#" + array.id;
               accessor.count = (UInt64)values.Length;
               accessor.stride = 1;

               // Add interpolation param
               {
                  param param = new param();
                  accessor.param = accessor.param.Append(param);

                  param.name = "INTERPOLATION";
                  param.type = "name";
               }
            }

            // Add sampler
            String samplerId = null;
            if ( frames.Count > 0 )
            {
               sampler sampler = new sampler();
               animation.Items = animation.Items.Append(sampler);

               sampler.id = MakeUnique(animation.id + "-sampler", usedNames);
               samplerId = sampler.id;

               // Add input input
               if ( inputSourceId != null )
               {
                  InputLocal input = new InputLocal();
                  sampler.input = sampler.input.Append(input);

                  input.semantic = "INPUT";
                  input.source = "#" + inputSourceId;
               }

               // Add output input
               if ( outputSourceId != null )
               {
                  InputLocal input = new InputLocal();
                  sampler.input = sampler.input.Append(input);

                  input.semantic = "OUTPUT";
                  input.source = "#" + outputSourceId;
               }

               // Add interpolation input
               if ( interpolationSourceId != null )
               {
                  InputLocal input = new InputLocal();
                  sampler.input = sampler.input.Append(input);

                  input.semantic = "INTERPOLATION";
                  input.source = "#" + interpolationSourceId;
               }
            }

            // Add channel
            if ( samplerId != null )
            {
               channel channel = new channel();
               animation.Items = animation.Items.Append(channel);

               channel.source = "#" + samplerId;
               channel.target = node.Id + "/transform";
            }
         }

         foreach ( VisualNode childNode in node.Children )
         {
            AddBoneAnimation(an8, usedNames, library, childNode, jointangles);
         }
      }

      static SortedDictionary<Double, An8.matrix> CalculateKeyFrames(
         ANIM8OR an8,
         VisualNode bNode,
         List<jointangle> jointangles)
      {
         SortedDictionary<Double, An8.matrix> frames =
            new SortedDictionary<Double, An8.matrix>();

         List<Double> frameNumbers = new List<Double>();

         foreach ( jointangle jointangle in jointangles )
         {
            foreach ( floatkey key in jointangle.track?.floatkey ??
               new floatkey[0] )
            {
               if ( !frameNumbers.Contains(key.frame) )
               {
                  frameNumbers.Add(key.frame);
               }
            }
         }

         frameNumbers.Sort();

         Double framesPerSecond = 24;

         if ( an8.environment?.limitplayback != null )
         {
            if ( an8.environment.framerate != null )
            {
               framesPerSecond = an8.environment.framerate.text;
            }
         }

         foreach ( Double frameNumber in frameNumbers )
         {
            Double xDegrees = InterpolateAngle(jointangles, "X", frameNumber);
            Double yDegrees = InterpolateAngle(jointangles, "Y", frameNumber);
            Double zDegrees = InterpolateAngle(jointangles, "Z", frameNumber);

            // Anim8or rotates counter-clockwise
            quaternion quatX = new quaternion(new point(1, 0, 0), xDegrees * Math.PI / 180);
            quaternion quatY = new quaternion(new point(0, 1, 0), yDegrees * Math.PI / 180);
            quaternion quatZ = new quaternion(new point(0, 0, 1), zDegrees * Math.PI / 180);

            // Anim8or seems to apply X then Z then Y in sequence mode
            quaternion sequenceQuat = quatX.Rotate(quatZ).Rotate(quatY);
            An8.matrix sequenceMat = new An8.matrix(new point(), sequenceQuat, 1);

            Double time = frameNumber / framesPerSecond;
            An8.matrix matrix = bNode.Matrix.Multiply(sequenceMat);

            frames.Add(time, matrix);
         }

         return frames;
      }

      static Double InterpolateAngle(
         List<jointangle> jointangles,
         String axis,
         Double time)
      {
         jointangle jointangle = jointangles.Find(j => j.axis == axis);
         floatkey start = null, end = null;

         foreach ( floatkey key in jointangle?.track?.floatkey ??
            new floatkey[0] )
         {
            if ( key.frame <= time &&
               (start == null || start.frame < key.frame) )
            {
               start = key;
            }

            if ( key.frame >= time &&
               (end == null || end.frame > key.frame) )
            {
               end = key;
            }
         }

         if ( start == null )
         {
            if ( end == null )
            {
               return 0; // no key frames for this axis?
            }
            else
            {
               return end.value; // only an end angle
            }
         }
         else if ( end == null || end == start )
         {
            return start.value; // only a start angle
         }
         else
         {
            return (end.value - start.value) / (end.frame - start.frame);
         }
      }
      #endregion

      #region library_visual_scenes
      static void CreateLibraryVisualScenes(
         ANIM8OR an8,
         COLLADA dae,
         VisualNode parentNode)
      {
         library_visual_scenes library = new library_visual_scenes();
         dae.Items = dae.Items.Append(library);

         visual_scene scene = new visual_scene();
         library.visual_scene = library.visual_scene.Append(scene);

         scene.id = parentNode.Id;
         scene.name = parentNode.Id;

         foreach ( VisualNode childNode in parentNode.Children )
         {
            node node = ConvertNode(childNode);
            scene.node = scene.node.Append(node);
         }
      }

      static node ConvertNode(VisualNode visualNode)
      {
         node node = new node();
         node.id = visualNode.Id;
         node.name = visualNode.Id;
         node.sid = visualNode.Id;
         node.type = visualNode.Tag is bone1 ? NodeType.JOINT : NodeType.NODE;

         Dae.V141.matrix matrix = new Dae.V141.matrix();
         node.Items = node.Items.Append(matrix);

         matrix.sid = "transform";
         matrix.Values = visualNode.Matrix.GetEnumerator().ToArray();

         node.ItemsElementName = new ItemsChoiceType7[]
         {
               ItemsChoiceType7.matrix,
         };

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

            if ( visualNode.SkeletonId != null )
            {
               i.skeleton = new String[] { "#" + visualNode.SkeletonId };
            }
         }

         foreach ( VisualNode childVisualNode in visualNode.Children )
         {
            node node2 = ConvertNode(childVisualNode);
            node.node1 = node.node1.Append(node2);
         }

         return node;
      }
      #endregion

      #region scene
      static void CreateScene(COLLADA dae, ANIM8OR an8, VisualNode parentNode)
      {
         dae.scene = new COLLADAScene();
         dae.scene.instance_visual_scene = new InstanceWithExtra();
         dae.scene.instance_visual_scene.url = "#" + parentNode.Id;
      }
      #endregion

      #region common
      public class Result
      {
         public enum An8Mode
         {
            Object,
            Figure,
            Sequence,
            Scene,
         }

         public An8Mode Mode;
         public String Name;
         public COLLADA Dae;
      }

      class VisualNode
      {
         public VisualNode(String id, An8.matrix matrix, Object tag)
         {
            Id = id;
            Matrix = matrix;
            Tag = tag;
            Children = new List<VisualNode>();
         }

         public void Link(VisualNode child)
         {
            child.Parent = this;
            Children.Add(child);
         }

         public An8.matrix BindMatrix()
         {
            An8.matrix matrix = Matrix;
            VisualNode iterator = Parent;

            while ( iterator != null )
            {
               matrix = iterator.Matrix.Multiply(matrix);

               iterator = iterator.Parent;
            }

            return matrix;
         }

         public VisualNode Find(Func<VisualNode, Boolean> condition)
         {
            if ( condition(this) )
            {
               return this;
            }

            foreach ( VisualNode child in Children )
            {
               VisualNode match = child.Find(condition);

               if ( match != null )
               {
                  return match;
               }
            }

            return null;
         }

         public IEnumerable<VisualNode> FindAll(
            Func<VisualNode, Boolean> condition)
         {
            if ( condition(this) )
            {
               yield return this;
            }

            foreach ( VisualNode child in Children )
            {
               foreach ( VisualNode match in child.FindAll(condition) )
               {
                  yield return match;
               }
            }
         }

         public String Id;
         public An8.matrix Matrix;
         public Object Tag;
         public An8.V100.mesh Mesh;
         public String GeometryId;
         public String ControllerId;
         public String SkeletonId;
         public VisualNode Parent;
         public List<VisualNode> Children;
      }

      static readonly Char[] NUM_CHARS = new Char[]
      {
         '0',
         '1',
         '2',
         '3',
         '4',
         '5',
         '6',
         '7',
         '8',
         '9',
      };

      static String MakeUnique(String name, List<String> usedNames)
      {
         // TODO: Autodesk seems to have problems with spaces. Should spaces
         // be allowed?
         name = Regex.Replace(name ?? "Unnamed", @"\s+", "_");

         Int32 number = 0;

         while ( usedNames.Contains(name) )
         {
            name = $"{name.TrimEnd(NUM_CHARS)}{++number:00}";
         }

         usedNames.Add(name);
         return name;
      }
      #endregion
   }
}
