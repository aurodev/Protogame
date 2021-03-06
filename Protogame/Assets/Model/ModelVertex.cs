﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Protogame
{
    /// <summary>
    /// Represents a model vertex with all available data from the model.
    /// <para>
    /// Models may store different data in different areas depending on what
    /// shader is intended to be used with the model.  For example, some
    /// models might specify vertex colors, and others might use textures
    /// and bump maps.
    /// </para>
    /// </summary>
    /// <module>Assets</module>
    public struct ModelVertex
    {
        public ModelVertex(
            Vector3? position,
            Vector3? normal,
            Vector3? tangent,
            Vector3? bitangent,
            Color[] colors,
            Vector2[] texCoordsUV,
            Vector3[] texCoordsUVW,
            Byte4? boneIndicies,
            Vector4? boneWeights)
        {
            Position = position;
            Normal = normal;
            Tangent = tangent;
            BiTangent = bitangent;
            Colors = colors;
            TexCoordsUV = texCoordsUV;
            TexCoordsUVW = texCoordsUVW;
            BoneIndices = boneIndicies;
            BoneWeights = boneWeights;
        }

        /// <summary>
        /// The 3D position in space of this vertex, if specified.
        /// </summary>
        public Vector3? Position;

        /// <summary>
        /// The normal of the vertex, if specified.
        /// </summary>
        public Vector3? Normal;

        /// <summary>
        /// The tangent of the vertex, if specified.
        /// </summary>
        public Vector3? Tangent;

        /// <summary>
        /// The bitangent of the vertex, if specified.
        /// </summary>
        public Vector3? BiTangent;

        /// <summary>
        /// The color channels associated with the vertex.  A vertex
        /// can have zero or more color channels.
        /// </summary>
        public Color[] Colors;

        /// <summary>
        /// The 2D texture coordinates associated with the vertex.  These
        /// texture coordinates are often refered to as UV-coordinates.  A
        /// vertex can have zero or more texture coordinate channels.
        /// </summary>
        public Vector2[] TexCoordsUV;

        /// <summary>
        /// The 3D texture coordinates associated with the vertex.  These
        /// texture coordinates are often refered to as UVW-coordinates.  
        /// Often you won't use these; they're only used if the model is
        /// being rendered using a 3D texture or cube, or if you're storing
        /// non-texture data in these channels.  A vertex can have zero or
        /// more texture coordinate channels.
        /// </summary>
        public Vector3[] TexCoordsUVW;

        /// <summary>
        /// The indicies of the bones associated with this vertex.  This
        /// data is calculated by the model importer based on the bones
        /// configured in the model.  If there are no bones in the model,
        /// or this vertex isn't affected by bones, this value is null.
        /// </summary>
        public Byte4? BoneIndices;

        /// <summary>
        /// The weights of the bones associated with this vertex.  This
        /// data is calculated by the model importer based on the bones
        /// configured in the model.  If there are no bones in the model,
        /// or this vertex isn't affected by bones, this value is null.
        /// </summary>
        public Vector4? BoneWeights;

        /// <summary>
        /// Provides a very basic representation of the vertex (just the
        /// position information).
        /// </summary>
        /// <returns>A string representation of the model vertex.</returns>
        public override string ToString()
        {
            return Position?.ToString() ?? "(no position)";
        }

        /// <summary>
        /// Transforms the current model vertex by the matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public ModelVertex Transform(Matrix matrix)
        {
            return new ModelVertex(
                Position == null ? null : (Vector3?)Vector3.Transform(Position.Value, matrix),
                Normal == null ? null : (Vector3?)Vector3.TransformNormal(Normal.Value, matrix),
                Tangent == null ? null : (Vector3?)Vector3.TransformNormal(Tangent.Value, matrix),
                BiTangent == null ? null : (Vector3?)Vector3.TransformNormal(BiTangent.Value, matrix),
                Colors,
                TexCoordsUV,
                TexCoordsUVW,
                BoneIndices,
                BoneWeights);
        }
    }
}
