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
      static L[] Append<L, T>(L[] list, T item) where L : class
      {
         if ( list != null )
         {
            // Add to the array
            L[] newList = new L[list.LongLength + 1];
            Array.Copy(list, newList, list.LongLength);
            newList[list.LongLength] = item as L;
            return newList;
         }
         else
         {
            // Create a new array
            return new L[] { item as L };
         }
      }

      /// <summary>
      /// In order to guarantee unique names when exporting, convert the '.'
      /// character into "._". We'll use '.' to separate the parent's name from
      /// child's name. For example:
      ///  * "object01"
      ///     * "mesh01"   // becomes "object01.mesh01"
      ///     * ".mesh01"  // becomes "object01.._mesh01"
      ///     * "_mesh01"  // becomes "object01._mesh01"
      ///     * "._mesh01" // becomes "object01..__mesh01;
      ///  * "object01."
      ///     * "mesh01"   // becomes "object01._.mesh01"
      ///     * ".mesh01"  // becomes "object01._.._mesh01"
      ///     * "_mesh01"  // becomes "object01._._mesh01"
      ///     * "._mesh01" // becomes "object01._..__mesh01"
      ///  * "object01_"
      ///     * "mesh01"   // becomes "object01_.mesh01"
      ///     * ".mesh01"  // becomes "object01_.._mesh01"
      ///     * "_mesh01"  // becomes "object01_._mesh01"
      ///     * "._mesh01" // becomes "object01_..__mesh01"
      ///  * "object01._"
      ///     * "mesh01"   // becomes "object01.__.mesh01"
      ///     * ".mesh01"  // becomes "object01.__.._mesh01"
      ///     * "_mesh01"  // becomes "object01.__._mesh01"
      ///     * "._mesh01" // becomes "object01.__..__mesh01"
      /// </summary>
      static String Escape(String name)
      {
         return name?.Replace(".", "._");
      }

      static String Combine(String parentName, String childName)
      {
         return parentName + "." + childName;
      }
      #endregion

      #region asset
      static void CreateAsset(COLLADA dae, ANIM8OR an8)
      {
         AssemblyName assemblyName = typeof(FormatConverter).Assembly.GetName();

         dae.asset = new asset();
         dae.asset.contributor = new assetContributor[1];
         dae.asset.contributor[0] = new assetContributor();
         dae.asset.contributor[0].author = "Anim8or v" +
            an8?.header?.version?.text ?? "0.0.0" + " build " +
            an8?.header?.build?.text ?? "1970.1.1";
         dae.asset.contributor[0].authoring_tool =
            assemblyName.Name + " v" + assemblyName.Version.ToString(3);
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
         library_geometries library_geometries = new library_geometries();
         dae.Items = Append(dae.Items, library_geometries);

         foreach ( @object o in an8?.@object ?? new @object[0] )
         {
            void AddModifier(VisualNode node, String parentName, modifier2 m)
            {
               String thisName = Combine(parentName, Escape(m.name?.text));
               // TODO: I think the modifier base only affects the modifier.
               VisualNode thisNode = new VisualNode(thisName, null, false);
               node.ChildrenNodes.Add(thisNode);

               if ( m.mesh != null )
               {
                  An8.V100.mesh m2 = m.mesh;
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.sphere != null )
               {
                  An8.V100.mesh m2 = An8Sphere.Convert(m.sphere);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.cylinder != null )
               {
                  An8.V100.mesh m2 = An8Cylinder.Convert(m.cylinder);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.cube != null )
               {
                  An8.V100.mesh m2 = An8Cube.Convert(m.cube);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.subdivision != null )
               {
                  An8.V100.mesh m2 = An8Subdivision.Convert(m.subdivision);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.pathcom != null )
               {
                  An8.V100.mesh m2 = An8PathCom.Convert(m.pathcom);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.textcom != null )
               {
                  An8.V100.mesh m2 = An8TextCom.Convert(m.textcom);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.modifier != null )
               {
                  AddModifier(thisNode, thisName, m.modifier);
               }

               if ( m.image != null )
               {
                  An8.V100.mesh m2 = An8Image.Convert(m.image);
                  geometry geometry = Convert(thisName, m2);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m2.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               if ( m.group != null )
               {
                  AddGroup(thisNode, thisName, m.group);
               }
            }

            void AddGroup(VisualNode node, String parentName, group2 g)
            {
               String thisName = Combine(parentName, Escape(g.name?.text));
               VisualNode thisNode = new VisualNode(
                  thisName,
                  An8Math.Transform(g.@base),
                  false);
               node.ChildrenNodes.Add(thisNode);

               foreach ( An8.V100.mesh m in g.mesh ?? new An8.V100.mesh[0] )
               {
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( An8.V100.sphere s in g.sphere ??
                         new An8.V100.sphere[0] )
               {
                  An8.V100.mesh m = An8Sphere.Convert(s);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( An8.V100.cylinder c in g.cylinder ??
                         new An8.V100.cylinder[0] )
               {
                  An8.V100.mesh m = An8Cylinder.Convert(c);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( cube c in g.cube ?? new cube[0] )
               {
                  An8.V100.mesh m = An8Cube.Convert(c);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( subdivision s in g.subdivision ?? new subdivision[0] )
               {
                  An8.V100.mesh m = An8Subdivision.Convert(s);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( pathcom p in g.pathcom ?? new pathcom[0] )
               {
                  An8.V100.mesh m = An8PathCom.Convert(p);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( textcom t in g.textcom ?? new textcom[0] )
               {
                  An8.V100.mesh m = An8TextCom.Convert(t);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( modifier2 m in g.modifier ?? new modifier2[0] )
               {
                  AddModifier(thisNode, thisName, m);
               }

               foreach ( An8.V100.image i in g.image ?? new An8.V100.image[0] )
               {
                  An8.V100.mesh m = An8Image.Convert(i);
                  geometry geometry = Convert(thisName, m);

                  thisNode.ChildrenNodes.Add(new VisualNode(
                     geometry.id,
                     An8Math.Transform(m.@base)));

                  library_geometries.geometry = Append(
                     library_geometries.geometry,
                     geometry);
               }

               foreach ( group2 g2 in g.group ?? new group2[0] )
               {
                  AddGroup(thisNode, thisName, g2);
               }
            }

            String name = Escape(o.name);

            foreach ( An8.V100.mesh m in o.mesh ?? new An8.V100.mesh[0] )
            {
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( An8.V100.sphere s in o.sphere ?? new An8.V100.sphere[0])
            {
               An8.V100.mesh m = An8Sphere.Convert(s);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( An8.V100.cylinder c in o.cylinder ??
                      new An8.V100.cylinder[0] )
            {
               An8.V100.mesh m = An8Cylinder.Convert(c);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( cube c in o.cube ?? new cube[0] )
            {
               An8.V100.mesh m = An8Cube.Convert(c);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( subdivision s in o.subdivision ?? new subdivision[0] )
            {
               An8.V100.mesh m = An8Subdivision.Convert(s);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( pathcom p in o.pathcom ?? new pathcom[0] )
            {
               An8.V100.mesh m = An8PathCom.Convert(p);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( textcom t in o.textcom ?? new textcom[0] )
            {
               An8.V100.mesh m = An8TextCom.Convert(t);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( modifier2 m in o.modifier ?? new modifier2[0] )
            {
               AddModifier(sVisualNode, name, m);
            }

            foreach ( An8.V100.image i in o.image ?? new An8.V100.image[0] )
            {
               An8.V100.mesh m = An8Image.Convert(i);
               geometry geometry = Convert(name, m);

               sVisualNode.ChildrenNodes.Add(new VisualNode(
                  geometry.id,
                  An8Math.Transform(m.@base)));

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);
            }

            foreach ( group2 g in o.group ?? new group2[0] )
            {
               AddGroup(sVisualNode, name, g);
            }
         }
      }

      static geometry Convert(String parentName, An8.V100.mesh m)
      {
         geometry geometry = new geometry();
         geometry.id = Combine(parentName, Escape(m.name?.text));
         geometry.name = geometry.id;

         Dae.V141.mesh mesh = new Dae.V141.mesh();
         geometry.Item = mesh;

         // Add source for points
         String pointsSourceId = null;
         if ( m.points?.point != null )
         {
            source source = new source();
            mesh.source = Append(mesh.source, source);

            source.id = geometry.id + "-positions";
            pointsSourceId = source.id;

            float_array float_array = new float_array();
            source.Item = float_array;

            float_array.id = source.id + "-array";

            Double[] values = new Double[m.points.point.LongLength * 3];

            for ( Int64 i = 0; i < m.points.point.LongLength; i++ )
            {
               values[i * 3 + 0] = m.points.point[i].x;
               values[i * 3 + 1] = m.points.point[i].y;
               values[i * 3 + 2] = m.points.point[i].z;
            }

            float_array.count = (UInt64)values.LongLength;
            float_array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + float_array.id;
            accessor.count = (UInt64)m.points.point.LongLength;
            accessor.stride = 3;

            // Add x param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "X";
               param.type = "float";
            }

            // Add y param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "Y";
               param.type = "float";
            }

            // Add z param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "Z";
               param.type = "float";
            }
         }

         // Add source for normals
         String normalsSourceId = null;
         if ( m.normals?.point != null )
         {
            source source = new source();
            mesh.source = Append(mesh.source, source);

            source.id = geometry.id + "-normals";
            normalsSourceId = source.id;

            float_array float_array = new float_array();
            source.Item = float_array;

            float_array.id = source.id + "-array";

            Double[] values = new Double[m.normals.point.LongLength * 3];

            for ( Int64 i = 0; i < m.normals.point.LongLength; i++ )
            {
               values[i * 3 + 0] = m.normals.point[i].x;
               values[i * 3 + 1] = m.normals.point[i].y;
               values[i * 3 + 2] = m.normals.point[i].z;
            }

            float_array.count = (UInt64)values.LongLength;
            float_array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + float_array.id;
            accessor.count = (UInt64)m.normals.point.LongLength;
            accessor.stride = 3;

            // Add x param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "X";
               param.type = "float";
            }

            // Add y param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "Y";
               param.type = "float";
            }

            // Add z param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "Z";
               param.type = "float";
            }
         }

         // Add source for texcoords
         String texcoordsSourceId = null;
         if ( m.texcoords?.texcoord != null )
         {
            source source = new source();
            mesh.source = Append(mesh.source, source);

            source.id = geometry.id + "-map-0";
            texcoordsSourceId = source.id;

            float_array float_array = new float_array();
            source.Item = float_array;

            float_array.id = source.id + "-array";

            Double[] values = new Double[m.texcoords.texcoord.LongLength * 2];

            for ( Int64 i = 0; i < m.texcoords.texcoord.LongLength; i++ )
            {
               values[i * 2 + 0] = m.texcoords.texcoord[i].u;
               values[i * 2 + 1] = m.texcoords.texcoord[i].v;
            }

            float_array.count = (UInt64)values.LongLength;
            float_array.Values = values;

            source.technique_common = new sourceTechnique_common();

            accessor accessor = new accessor();
            source.technique_common.accessor = accessor;

            accessor.source = "#" + float_array.id;
            accessor.count = (UInt64)m.texcoords.texcoord.LongLength;
            accessor.stride = 2;

            // Add s param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "S";
               param.type = "float";
            }

            // Add t param
            {
               param param = new param();
               accessor.param = Append(accessor.param, param);

               param.name = "T";
               param.type = "float";
            }
         }

         if ( m.points?.point != null &&
              m.faces?.facedata != null )
         {
            // Add vertices
            String verticesSourceId = null;
            {
               mesh.vertices = new vertices();
               mesh.vertices.id = geometry.id + "-vertices";
               verticesSourceId = mesh.vertices.id;

               InputLocal input = new InputLocal();
               mesh.vertices.input = Append(mesh.vertices.input, input);

               input.semantic = "POSITION";
               input.source = "#" + pointsSourceId;
            }

            // Add polylist
            polylist polylist = new polylist();
            mesh.Items = Append(mesh.Items, polylist);

            // TODO: Set polylist.material

            polylist.count = (UInt64)m.faces.facedata.LongLength;

            UInt64 offset = 0;

            // Add vertex input
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = Append(polylist.input, input);

               input.semantic = "VERTEX";
               input.source = "#" + verticesSourceId;
               input.offset = offset++;
            }

            // Add normal input
            if ( m.normals?.point != null )
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = Append(polylist.input, input);

               input.semantic = "NORMAL";
               input.source = "#" + normalsSourceId;
               input.offset = offset++;
            }

            // Add texcoord input
            if ( m.texcoords?.texcoord != null )
            {
               InputLocalOffset input = new InputLocalOffset();
               polylist.input = Append(polylist.input, input);

               input.semantic = "TEXCOORD";
               input.source = "#" + texcoordsSourceId;
               input.offset = offset++;
               input.set = 0;
            }

            // Add face vertex counts
            {
               StringBuilder sb = new StringBuilder();

               for ( Int64 i = 0; i < m.faces.facedata.LongLength; i++ )
               {
                  // Note: We could use 'numpoints', but pointdata
                  // contains the actual list of points, so it is safer.
                  sb.Append(m.faces.facedata[i].pointdata.LongLength);

                  if ( i < m.faces.facedata.LongLength - 1 )
                  {
                     sb.Append(' ');
                  }
               }

               polylist.vcount = sb.ToString();
            }

            // Add indices
            {
               StringBuilder sb = new StringBuilder();

               for ( Int64 i = 0; i < m.faces.facedata.LongLength; i++ )
               {
                  // Note: An8 specifies faces in clockwise order, while
                  // Dae specifies faces in counter-clockwise order.
                  for ( Int64 j = m.faces.facedata[i].pointdata.LongLength - 1;
                        j >= 0;
                        j-- )
                  {
                     sb.Append(m.faces.facedata[i].pointdata[j].pointindex);

                     // Add normal index if needed
                     if ( m.normals?.point != null )
                     {
                        sb.Append(' ');

                        // Note: An8 supports enabling/disabling normals
                        // per face. Dae only supports enabling/disabling
                        // normals for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(
                           m.faces.facedata[i].pointdata[j].normalindex);
                     }

                     // Add texcoord index if needed
                     if ( m.texcoords?.texcoord != null )
                     {
                        sb.Append(' ');

                        // Note: An8 supports enabling/disabling textures
                        // per face. Dae only supports enabling/disabling
                        // textures for the whole mesh. For that reason,
                        // we may incorrectly use index 0 sometimes.
                        sb.Append(
                           m.faces.facedata[i].pointdata[j].texcoordindex);
                     }

                     if ( i < m.faces.facedata.LongLength - 1 ||
                          j > 0 )
                     {
                        sb.Append(' ');
                     }
                  }
               }

               polylist.p = sb.ToString();
            }
         }

         return geometry;
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
         // TODO: Implement
      }
      #endregion

      #region library_visual_scenes
      class VisualNode
      {
         public VisualNode(
            String id,
            Double[] matrix = null,
            Boolean hasGeometry = true)
         {
            Id = id;
            Matrix = matrix;
            HasGeometry = hasGeometry;
            ChildrenNodes = new List<VisualNode>();
         }
         public String Id;
         public Double[] Matrix;
         public Boolean HasGeometry;
         public List<VisualNode> ChildrenNodes;
      }
      static VisualNode sVisualNode = new VisualNode("root", null, false);

      static void CreateLibraryVisualScenes(COLLADA dae, ANIM8OR an8)
      {
         library_visual_scenes library_visual_scenes =
            new library_visual_scenes();
         dae.Items = Append(dae.Items, library_visual_scenes);
         visual_scene visual_scene = new visual_scene();
         library_visual_scenes.visual_scene = Append(
            library_visual_scenes.visual_scene,
            visual_scene);
         visual_scene.id = sVisualNode.Id;
         visual_scene.name = visual_scene.id;

         void AddNode(VisualNode visualNode, node parentNode = null)
         {
            node node = new node();

            if ( parentNode == null )
            {
               visual_scene.node = Append(visual_scene.node, node);
            }
            else
            {
               parentNode.node1 = Append(parentNode.node1, node);
            }

            node.id = visualNode.Id;
            node.name = node.id;
            node.sid = node.id;

            if ( visualNode.HasGeometry )
            {
               instance_geometry instance_geometry = new instance_geometry();
               node.instance_geometry = Append(
                  node.instance_geometry,
                  instance_geometry);
               instance_geometry.url = "#" + node.id;
            }

            if ( visualNode.Matrix?.Length == 16 )
            {
               matrix matrix = new matrix();
               node.Items = Append(node.Items, matrix);
               matrix.Values = visualNode.Matrix;
               node.ItemsElementName = new ItemsChoiceType7[]
               {
                  ItemsChoiceType7.matrix,
               };
            }

            foreach ( VisualNode childNode in visualNode.ChildrenNodes )
            {
               AddNode(childNode, node);
            }
         }

         AddNode(sVisualNode);
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
