// Copyright © 2023 Contingent Games.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace Anim8orTransl8or
{
   public class VisualNode
   {
      public VisualNode(String nodeId, An8.matrix matrix, Object tag)
      {
         NodeId = nodeId;
         Matrix = matrix;
         Tag = tag;
         MaterialIds = new List<String>();
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
            matrix = iterator.Matrix.Transform(matrix);

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

      public String NodeId;
      public An8.matrix Matrix;
      public Object Tag;
      public An8.V100.mesh Mesh;
      public String LightId;
      public String GeometryId;
      public List<String> MaterialIds;
      public String ControllerId;
      public String SkeletonId;
      public VisualNode Parent;
      public List<VisualNode> Children;
   }
}
