using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using System;
using System.Reflection;
using System.Text;

namespace Anim8orTransl8or
{
   public class Converter
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
      #endregion

      #region asset
      static void CreateAsset(COLLADA dae, ANIM8OR an8)
      {
         AssemblyName assemblyName = typeof(Converter).Assembly.GetName();

         dae.asset = new asset();
         dae.asset.contributor = new assetContributor[1];
         dae.asset.contributor[0] = new assetContributor();
         dae.asset.contributor[0].author = "Anim8or Transl8or User";
         dae.asset.contributor[0].authoring_tool =
            assemblyName.Name + " v" + assemblyName.Version.ToString(3);
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
            foreach ( An8.V100.mesh m in o.mesh ?? new An8.V100.mesh[0] )
            {
               geometry geometry = new geometry();

               library_geometries.geometry = Append(
                  library_geometries.geometry,
                  geometry);

               geometry.id = m.name.text + "-mesh";
               HACK_geometryId = geometry.id;

               geometry.name = m.name.text;

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
                        for ( Int64 j = m.faces.facedata[i].pointdata.LongLength - 1; j >= 0; j-- )
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
                              sb.Append(m.faces.facedata[i].pointdata[j].normalindex);
                           }

                           // Add texcoord index if needed
                           if ( m.texcoords?.texcoord != null )
                           {
                              sb.Append(' ');

                              // Note: An8 supports enabling/disabling textures
                              // per face. Dae only supports enabling/disabling
                              // textures for the whole mesh. For that reason,
                              // we may incorrectly use index 0 sometimes.
                              sb.Append(m.faces.facedata[i].pointdata[j].texcoordindex);
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
            }
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
         // TODO: Implement
      }
      #endregion

      #region library_visual_scenes
      // HACK: Just to see some geometry
      static String HACK_visualSceneId = null;
      static String HACK_geometryId = null;

      static void CreateLibraryVisualScenes(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
         if ( HACK_geometryId != null )
         {
            library_visual_scenes library_visual_scenes = new library_visual_scenes();
            dae.Items = Append(dae.Items, library_visual_scenes);

            visual_scene visual_scene = new visual_scene();
            library_visual_scenes.visual_scene = Append(library_visual_scenes.visual_scene, visual_scene);

            visual_scene.id = "test";
            HACK_visualSceneId = visual_scene.id;

            visual_scene.name = visual_scene.id;

            node node = new node();
            visual_scene.node = Append(visual_scene.node, node);

            node.id = HACK_geometryId.Replace("-mesh", "");
            node.name = node.id;
            node.sid = node.id;

            instance_geometry instance_geometry = new instance_geometry();
            node.instance_geometry = Append(node.instance_geometry, instance_geometry);

            instance_geometry.url = "#" + HACK_geometryId;
         }
      }
      #endregion

      #region scene
      static void CreateScene(COLLADA dae, ANIM8OR an8)
      {
         // TODO: Implement
         if ( HACK_visualSceneId != null )
         {
            dae.scene = new COLLADAScene();
            dae.scene.instance_visual_scene = new InstanceWithExtra();
            dae.scene.instance_visual_scene.url = "#" + HACK_visualSceneId;

         }
      }
      #endregion

   }
}
