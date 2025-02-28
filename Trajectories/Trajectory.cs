﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trajectories
{
    public class Trajectory : IList<Position>, ICloneable
    {
        #region MEMBERS
        #region LINEAR_TRAJECTORIES

        /// <summary>
        /// The x axis for the vector list position.
        /// </summary>
        public Vector<double> X { get; set; }

        /// <summary>
        /// The y axis for the vector list  position.
        /// </summary>
        public Vector<double> Y { get; set; }

        /// <summary>
        /// The z axis for the vector list position.
        /// </summary>
        public Vector<double> Z { get; set; }
        #endregion LINEAR_TRAJECTORIES

        #region ROTATION_TRAJECTORIES
        /// <summary>
        /// The x rotation axis for the vector list  position.
        /// </summary>
        public Vector<double> RX { get; set; }

        /// <summary>
        /// The y rotation axis for the vector list  position.
        /// </summary>
        public Vector<double> RY { get; set; }

        /// <summary>
        /// The z rotation axis for the vector list position.
        /// </summary>
        public Vector<double> RZ { get; set; }

        public int Count => (X != null) ? X.Count : 0;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        Position IList<Position>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Position this[int index]
        {
            get => new Position()
            {
                X = X[index],
                Y = Y[index],
                Z = Z[index],
                RX = RX[index],
                RY = RY[index],
                RZ = RZ[index],
            };
            set
            {
                X[index] = value.X;
                Y[index] = value.Y;
                Z[index] = value.Z;
                RX[index] = value.RX;
                RY[index] = value.RY;
                RZ[index] = value.RZ;
            }
        }
        #endregion ROTATION_TRAJECTORIES
        #endregion MEMBERS

        public Trajectory()
        {
        }

        //Maayan edit - adding another constructor to be used for sending a trajectory to the VR
        public Trajectory(Vector<double> x, Vector<double> y, Vector<double> z, Vector<double> rx, Vector<double> ry, Vector<double> rz)
        {
            X = x;
            Y = y;
            Z = z;
            RX = rx;
            RY = ry;
            RZ = rz;
        }

        public void InsertOriginPlace(bool forward = true)
        {

            Position originPoint = new Position() { X = 0, Y = 0, Z = 0, RX = 0, RY = 0, RZ = 0 };

            //todo:decide if to return new one or the input one (changed).
            if (!forward)
            {
                this.Add(originPoint);
            }
            else
            {
                this.Insert(0, originPoint);
            }
        }

        public int IndexOf(Position item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Position item)
        {
            List<double> x = X.ToList();
            List<double> y = Y.ToList();
            List<double> z = Z.ToList();
            List<double> rx = RX.ToList();
            List<double> ry = RY.ToList();
            List<double> rz = RZ.ToList();
            x.Insert(index, item.X);
            y.Insert(index, item.Y);
            z.Insert(index, item.Z);
            rx.Insert(index, item.RX);
            ry.Insert(index, item.RY);
            rz.Insert(index, item.RZ);

            X = Vector<double>.Build.Dense(x.ToArray());
            Y = Vector<double>.Build.Dense(y.ToArray());
            Z = Vector<double>.Build.Dense(z.ToArray());
            RX = Vector<double>.Build.Dense(rx.ToArray());
            RY = Vector<double>.Build.Dense(ry.ToArray());
            RZ = Vector<double>.Build.Dense(rz.ToArray());
        }

        public void RemoveAt(int index)
        {
            List<double> x = X.ToList();
            List<double> y = Y.ToList();
            List<double> z = Z.ToList();
            List<double> rx = RX.ToList();
            List<double> ry = RY.ToList();
            List<double> rz = RZ.ToList();
            x.RemoveAt(index);
            y.RemoveAt(index);
            z.RemoveAt(index);
            rx.RemoveAt(index);
            ry.RemoveAt(index);
            rz.RemoveAt(index);

            X = Vector<double>.Build.Dense(x.ToArray());
            Y = Vector<double>.Build.Dense(y.ToArray());
            Z = Vector<double>.Build.Dense(z.ToArray());
            RX = Vector<double>.Build.Dense(rx.ToArray());
            RY = Vector<double>.Build.Dense(ry.ToArray());
            RZ = Vector<double>.Build.Dense(rz.ToArray());
        }

        public void Add(Position item)
        {
            if (this.Count == 0)
            {
                X = Vector<double>.Build.Dense(1);
                Y = Vector<double>.Build.Dense(1);
                Z = Vector<double>.Build.Dense(1);
                RX = Vector<double>.Build.Dense(1);
                RY = Vector<double>.Build.Dense(1);
                RZ = Vector<double>.Build.Dense(1);

                X[0] = item.X;
                Y[0] = item.Y;
                Z[0] = item.Z;
                RX[0] = item.RX;
                RY[0] = item.RY;
                RZ[0] = item.RZ;
            }
            else
            {
                this.Insert(this.Count, item);
            }
        }

        public void Clear()
        {
            X.Clear();
            Y.Clear();
            Z.Clear();
            RX.Clear();
            RY.Clear();
            RZ.Clear();
        }

        public bool Contains(Position item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Position[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Position item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Position> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a new Trajectory (independent from current) reversed to the current Trajectory.
        /// </summary>
        /// <returns>The new  reversed Trajectory.</returns>
        public Trajectory ToReverse()
        {
            //the inversed trajectory to be returned.
            Trajectory inverseTrajectory = new Trajectory();

            //initialization for the trajectory to be retund.
            int length = this.Count;
            inverseTrajectory.X = Vector<double>.Build.Dense(length);
            inverseTrajectory.Y = Vector<double>.Build.Dense(length);
            inverseTrajectory.Z = Vector<double>.Build.Dense(length);
            inverseTrajectory.RX = Vector<double>.Build.Dense(length);
            inverseTrajectory.RY = Vector<double>.Build.Dense(length);
            inverseTrajectory.RZ = Vector<double>.Build.Dense(length);

            //inverse the original trajectory into the new trajectory.
            for (int i = 0; i < length; i++)
            {
                int index = length - 1 - i;

                inverseTrajectory.X[i] = this.X[index];
                inverseTrajectory.Y[i] = this.Y[index];
                inverseTrajectory.Z[i] = this.Z[index];
                inverseTrajectory.RX[i] = this.RX[index];
                inverseTrajectory.RY[i] = this.RY[index];
                inverseTrajectory.RZ[i] = this.RZ[index];
            }

            //return the inversed trajectory.
            return inverseTrajectory;
        }

        public Trajectory Clone()
        {
            Trajectory clonedTrajectory = new Trajectory();

            for (int i = 0; i < this.Count; i++)
            {
                clonedTrajectory.Add(this[i]);
            }

            return clonedTrajectory;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
