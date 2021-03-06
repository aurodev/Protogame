﻿// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="ITransform"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ITransform</interface_ref>
    public class DefaultTransform : ITransform
    {
        private bool _isSRTMatrix;

        private Vector3 _srtLocalPosition;

        private Quaternion _srtLocalRotation;

        private Vector3 _srtLocalScale;

        private Matrix _customLocalMatrix;

        private Matrix _cachedSRTLocalMatrix;

        private bool _isCachedSRTLocalMatrixUpToDate;

        private Matrix _cachedRTLocalMatrix;

        private bool _isCachedRTLocalMatrixUpToDate;

        public event EventHandler Modified;

        public DefaultTransform()
        {
            _isSRTMatrix = true;
            _srtLocalPosition = Vector3.Zero;
            _srtLocalRotation = Quaternion.Identity;
            _srtLocalScale = Vector3.One;
            _cachedSRTLocalMatrix = Matrix.Identity;
            _isCachedSRTLocalMatrixUpToDate = true;
            _cachedRTLocalMatrix = Matrix.Identity;
            _isCachedRTLocalMatrixUpToDate = true;
        }

        public void Assign(ITransform from)
        {
            if (from.IsSRTMatrix)
            {
                SetFromSRTMatrix(
                    from.LocalPosition,
                    from.LocalRotation,
                    from.LocalScale);
                Modified?.Invoke(this, new EventArgs());
            }
            else
            {
                SetFromCustomMatrix(
                    from.LocalMatrix);
                Modified?.Invoke(this, new EventArgs());
            }
        }

        #region Local Properties

        public Vector3 LocalPosition
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalPosition;
                }
                else
                {
                    throw new InvalidOperationException("Local position can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    if (_srtLocalPosition != value)
                    {
                        _srtLocalPosition = value;
                        _isCachedSRTLocalMatrixUpToDate = false;
                        _isCachedRTLocalMatrixUpToDate = false;
                        Modified?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Local position can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalRotation;
                }
                else
                {
                    throw new InvalidOperationException("Local rotation can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    if (_srtLocalRotation != value)
                    {
                        _srtLocalRotation = value;
                        _isCachedSRTLocalMatrixUpToDate = false;
                        _isCachedRTLocalMatrixUpToDate = false;
                        Modified?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Local rotation can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                if (_isSRTMatrix)
                {
                    return _srtLocalScale;
                }
                else
                {
                    throw new InvalidOperationException("Local scale can only be retrieved on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
            set
            {
                if (_isSRTMatrix)
                {
                    if (_srtLocalScale != value)
                    {
                        _srtLocalScale = value;
                        _isCachedSRTLocalMatrixUpToDate = false;
                        Modified?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Local scale can only be assigned on transforms derived from SRT (scale-rotate-transform) matrices.");
                }
            }
        }

        public Matrix LocalMatrix
        {
            get
            {
                if (_isSRTMatrix)
                {
                    if (!_isCachedSRTLocalMatrixUpToDate)
                    {
                        RecalculateSRTMatrixCache();
                    }

                    return _cachedSRTLocalMatrix;
                }
                else
                {
                    return _customLocalMatrix;
                }
            }
        }

        public Matrix LocalMatrixWithoutScale
        {
            get
            {
                if (_isSRTMatrix)
                {
                    if (!_isCachedRTLocalMatrixUpToDate)
                    {
                        RecalculateRTMatrixCache();
                    }

                    return _cachedRTLocalMatrix;
                }
                else
                {
                    throw new InvalidOperationException("It is not possible to remove the scaling component from a non-SRT matrix.");
                }
            }
        }

        public bool IsSRTMatrix
        {
            get { return _isSRTMatrix; }
        }

        #endregion

        #region Type of Matrix Methods

        public void ResetAsSRTMatrix()
        {
            _isSRTMatrix = true;
            _srtLocalPosition = Vector3.Zero;
            _srtLocalRotation = Quaternion.Identity;
            _srtLocalScale = Vector3.One;
            _isCachedSRTLocalMatrixUpToDate = false;
            _isCachedRTLocalMatrixUpToDate = false;
            Modified?.Invoke(this, new EventArgs());
        }

        public void SetFromSRTMatrix(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            _isSRTMatrix = true;
            _srtLocalPosition = localPosition;
            _srtLocalRotation = localRotation;
            _srtLocalScale = localScale;
            _isCachedSRTLocalMatrixUpToDate = false;
            _isCachedRTLocalMatrixUpToDate = false;
            Modified?.Invoke(this, new EventArgs());
        }

        public NetworkTransform SerializeToNetwork()
        {
            if (IsSRTMatrix)
            {
                var pos = LocalPosition;
                var rot = LocalRotation;
                var scale = LocalScale;

                return new NetworkTransform
                {
                    IsSRTMatrix = true,
                    SRTLocalPosition = new[] { pos.X, pos.Y, pos.Z },
                    SRTLocalRotation = new[] { rot.X, rot.Y, rot.Z, rot.W },
                    SRTLocalScale = new[] { scale.X, scale.Y, scale.Z },
                    CustomLocalMatrix = null
                };
            }
            else
            {
                var mat = LocalMatrix;

                return new NetworkTransform
                {
                    IsSRTMatrix = false,
                    SRTLocalPosition = null,
                    SRTLocalRotation = null,
                    SRTLocalScale = null,
                    CustomLocalMatrix = new[]
                    {
                        mat[0], mat[1], mat[2], mat[3],
                        mat[4], mat[5], mat[6], mat[7],
                        mat[8], mat[9], mat[10], mat[11],
                        mat[12], mat[13], mat[14], mat[15],
                    }
                };
            }
        }

        public void ResetAsCustomMatrix()
        {
            _isSRTMatrix = false;
            _customLocalMatrix = Matrix.Identity;
            Modified?.Invoke(this, new EventArgs());
        }

        public void SetFromCustomMatrix(Matrix localMatrix)
        {
            _isSRTMatrix = false;
            _customLocalMatrix = localMatrix;
            Modified?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Internal Caching

        private void RecalculateSRTMatrixCache()
        {
            if (!_isSRTMatrix)
            {
                throw new InvalidOperationException("Attempted to update SRT matrix cache for non-SRT matrix.");
            }

            if (_isCachedSRTLocalMatrixUpToDate)
            {
                throw new InvalidOperationException("Attempted to update SRT matrix cache when it's already up-to-date.  This would incur a performance penalty if the operation continued.");
            }

            _cachedSRTLocalMatrix =
                Matrix.CreateScale(_srtLocalScale) *
                Matrix.CreateFromQuaternion(_srtLocalRotation) *
                Matrix.CreateTranslation(_srtLocalPosition);
            _isCachedSRTLocalMatrixUpToDate = true;
        }

        private void RecalculateRTMatrixCache()
        {
            if (!_isSRTMatrix)
            {
                throw new InvalidOperationException("Attempted to update RT matrix cache for non-SRT matrix.");
            }

            if (_isCachedRTLocalMatrixUpToDate)
            {
                throw new InvalidOperationException("Attempted to update RT matrix cache when it's already up-to-date.  This would incur a performance penalty if the operation continued.");
            }

            _cachedRTLocalMatrix =
                Matrix.CreateFromQuaternion(_srtLocalRotation) *
                Matrix.CreateTranslation(_srtLocalPosition);
            _isCachedRTLocalMatrixUpToDate = true;
        }

        #endregion

        public override string ToString()
        {
            if (IsSRTMatrix)
            {
                return "T SRT P: " + LocalPosition + " R: " + LocalRotation + " S: " + LocalScale;
            }

            return "T CUS M: " + LocalMatrix;
        }
    }
}
