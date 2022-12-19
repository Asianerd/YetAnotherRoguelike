﻿//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherRoguelike.Graphics
{
    /// <summary>
    /// A Gaussian blur filter kernel class. A Gaussian blur filter kernel is
    /// perfectly symmetrical and linearly separable. This means we can split
    /// the full 2D filter kernel matrix into two smaller horizontal and
    /// vertical 1D filter kernel matrices and then perform the Gaussian blur
    /// in two passes. Contrary to what you might think performing the Gaussian
    /// blur in this way is actually faster than performing the Gaussian blur
    /// in a single pass using the full 2D filter kernel matrix.
    /// <para>
    /// The GaussianBlur class is intended to be used in conjunction with an
    /// HLSL Gaussian blur shader. The following code snippet shows a typical
    /// Effect file implementation of a Gaussian blur.
    /// <code>
    /// #define RADIUS  7
    /// #define KERNEL_SIZE (RADIUS * 2 + 1)
    ///
    /// float weights[KERNEL_SIZE];
    /// float2 offsets[KERNEL_SIZE];
    ///
    /// texture colorMapTexture;
    ///
    /// sampler2D colorMap = sampler_state
    /// {
    ///     Texture = <![CDATA[<colorMapTexture>;]]>
    ///     MipFilter = Linear;
    ///     MinFilter = Linear;
    ///     MagFilter = Linear;
    /// };
    ///
    /// float4 PS_GaussianBlur(float2 texCoord : TEXCOORD) : COLOR0
    /// {
    ///     float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
    ///
    ///     <![CDATA[for (int i = 0; i < KERNEL_SIZE; ++i)]]>
    ///         color += tex2D(colorMap, texCoord + offsets[i]) * weights[i];
    /// 
    ///     return color;
    /// }
    /// 
    /// technique GaussianBlur
    /// {
    ///     pass
    ///     {
    ///         PixelShader = compile ps_2_0 PS_GaussianBlur();
    ///     }
    /// }
    /// </code>
    /// The RADIUS constant in the effect file must match the radius value in
    /// the GaussianBlur class. The effect file's weights global variable
    /// corresponds to the GaussianBlur class' kernel field. The effect file's
    /// offsets global variable corresponds to the GaussianBlur class'
    /// offsetsHoriz and offsetsVert fields.
    /// </para>
    /// </summary>
    public class GaussianBlur
    {
        public static GaussianBlur Instance;

        private Effect Effect;
        private int _radius;
        private float _amount;
        private float _sigma;
        private float[] _kernel;
        private Vector2[] _offsetsHoriz;
        private Vector2[] _offsetsVert;

        /// <summary>
        /// Returns the radius of the Gaussian blur filter kernel in pixels.
        /// </summary>
        public int Radius
        {
            get { return _radius; }
        }

        /// <summary>
        /// Returns the blur amount. This value is used to calculate the
        /// Gaussian blur filter kernel's sigma value. Good values for this
        /// property are 2 and 3. 2 will give a more blurred result whilst 3
        /// will give a less blurred result with sharper details.
        /// </summary>
        public float Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// Returns the Gaussian blur filter's standard deviation.
        /// </summary>
        public float Sigma
        {
            get { return _sigma; }
        }

        /// <summary>
        /// Returns the Gaussian blur filter kernel matrix. Note that the
        /// kernel returned is for a 1D Gaussian blur filter kernel matrix
        /// intended to be used in a two pass Gaussian blur operation.
        /// </summary>
        public float[] Kernel
        {
            get { return _kernel; }
        }

        /// <summary>
        /// Returns the texture offsets used for the horizontal Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsX
        {
            get { return _offsetsHoriz; }
        }

        /// <summary>
        /// Returns the texture offsets used for the vertical Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsY
        {
            get { return _offsetsVert; }
        }

        public GaussianBlur()
        {
            Instance = this;

            Effect = Game.Instance.Content.Load<Effect>("Shaders/GaussianBlur");
        }

        /// <summary>
        /// Calculates the Gaussian blur filter kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate sigma.</param>
        public void ComputeKernel(int blurRadius, float blurAmount)
        {
            _radius = blurRadius;
            _amount = blurAmount;

            _kernel = null;
            _kernel = new float[_radius * 2 + 1];
            _sigma = _radius / _amount;

            float twoSigmaSquare = 2.0f * _sigma * _sigma;
            float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0.0f;
            float distance = 0.0f;
            int index = 0;

            for (int i = -_radius; i <= _radius; ++i)
            {
                distance = i * i;
                index = i + _radius;
                _kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += _kernel[index];
            }

            for (int i = 0; i < _kernel.Length; ++i)
                _kernel[i] /= total;
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        public void ComputeOffsets(float textureWidth, float textureHeight)
        {
            _offsetsHoriz = null;
            _offsetsHoriz = new Vector2[_radius * 2 + 1];

            _offsetsVert = null;
            _offsetsVert = new Vector2[_radius * 2 + 1];

            int index = 0;
            float xOffset = 1.0f / textureWidth;
            float yOffset = 1.0f / textureHeight;

            for (int i = -_radius; i <= _radius; ++i)
            {
                index = i + _radius;
                _offsetsHoriz[index] = new Vector2(i * xOffset, 0.0f);
                _offsetsVert[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <param name="renderTarget1">Stores the output from the horizontal blur pass.</param>
        /// <param name="renderTarget2">Stores the output from the vertical blur pass.</param>
        /// <param name="spriteBatch">Used to draw quads for the blur passes.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        public Texture2D PerformGaussianBlur(Texture2D srcTexture,
                                             RenderTarget2D renderTarget1,
                                             RenderTarget2D renderTarget2,
                                             SpriteBatch spriteBatch)
        {
            if (Effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            Texture2D outputTexture = null;
            //Rectangle srcRect = new Rectangle(0, 0, srcTexture.Width, srcTexture.Height);
            Rectangle destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
            Rectangle destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

            // Perform horizontal Gaussian blur.

            Game.graphics.GraphicsDevice.SetRenderTarget(renderTarget1);

            Effect.CurrentTechnique = Effect.Techniques["GaussianBlur"];
            Effect.Parameters["weights"].SetValue(_kernel);
            Effect.Parameters["colorMapTexture"].SetValue(srcTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, Effect);
            spriteBatch.Draw(srcTexture, destRect1, Color.White);
            spriteBatch.End();

            // Perform vertical Gaussian blur.

            Game.graphics.GraphicsDevice.SetRenderTarget(renderTarget2);
            outputTexture = (Texture2D)renderTarget1;

            Effect.Parameters["colorMapTexture"].SetValue(outputTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsVert);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, Effect);
            spriteBatch.Draw(outputTexture, destRect2, Color.White);
            spriteBatch.End();

            // Return the Gaussian blurred texture.

            Game.graphics.GraphicsDevice.SetRenderTarget(null);
            outputTexture = renderTarget2;

            return outputTexture;
        }

        public void RenderWithBlur(Texture2D srcTexture,
                                     RenderTarget2D renderTarget1,
                                     RenderTarget2D renderTarget2,
                                     SpriteBatch spriteBatch)
        {
            Texture2D outputTexture = null;
            Rectangle destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
            Rectangle destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

            // Perform horizontal Gaussian blur.

            Game.graphics.GraphicsDevice.SetRenderTarget(renderTarget1);

            Effect.CurrentTechnique = Effect.Techniques["GaussianBlur"];
            Effect.Parameters["weights"].SetValue(_kernel);
            Effect.Parameters["colorMapTexture"].SetValue(srcTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode:SpriteSortMode.Immediate, effect: Effect);
            spriteBatch.Draw(srcTexture, destRect1, Color.White);
            spriteBatch.End();

            // Perform vertical Gaussian blur.

            Game.graphics.GraphicsDevice.SetRenderTarget(null);
            outputTexture = (Texture2D)renderTarget1;

            Effect.Parameters["colorMapTexture"].SetValue(outputTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsVert);

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, effect: Effect);
            spriteBatch.Draw(outputTexture, destRect2, Color.White);
            spriteBatch.End();
            //Game.graphics.GraphicsDevice.SetRenderTarget(null);
        }
    }
}