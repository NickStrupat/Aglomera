﻿// ------------------------------------------
// <copyright file="Cluster.cs" company="Pedro Sequeira">
//     Some copyright
// </copyright>
// <summary>
//    Project: Agnes
//    Last updated: 2017/04/06
// 
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agnes
{
    public class Cluster<TInstance> :
        IEnumerable<TInstance>, IEquatable<Cluster<TInstance>>, IComparable<Cluster<TInstance>>
        where TInstance : IComparable<TInstance>
    {
        #region Static Fields & Constants

        public static readonly Cluster<TInstance> EmptySet = new Cluster<TInstance>(new List<TInstance>());

        #endregion

        #region Fields

        private readonly TInstance[] _cluster;

        private readonly int _hashCode;

        #endregion

        #region Properties & Indexers

        public uint Count => (uint) this._cluster.Length;

        public double Dissimilarity { get; }

        public Cluster<TInstance> Parent1 { get; }

        public Cluster<TInstance> Parent2 { get; }

        #endregion

        #region Constructors

        public Cluster(Cluster<TInstance> parent1, Cluster<TInstance> parent2, double dissimilarity)
        {
            this.Parent1 = parent1;
            this.Parent2 = parent2;
            this.Dissimilarity = dissimilarity;
            this._cluster = new TInstance[parent1._cluster.Length + parent2._cluster.Length];
            parent1._cluster.CopyTo(this._cluster, 0);
            parent2._cluster.CopyTo(this._cluster, parent1._cluster.Length);
        }

        public Cluster(TInstance instance, double dissimilarity = 0) : this(new[] {instance}, dissimilarity)
        {
        }

        public Cluster(IEnumerable<TInstance> instances, double dissimilarity = 0)
        {
            this.Dissimilarity = dissimilarity;
            this._cluster = instances as TInstance[] ?? instances.ToArray();
            this._hashCode = this.ProduceHashCode();
        }

        public Cluster(Cluster<TInstance> cluster)
        {
            this._cluster = cluster._cluster.ToArray();
            this.Parent1 = cluster.Parent1;
            this.Parent2 = cluster.Parent2;
            this.Dissimilarity = cluster.Dissimilarity;
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) ||
                    obj.GetType() == this.GetType() && this.Equals((Cluster<TInstance>) obj));
        }

        public override int GetHashCode() => this._hashCode;

        public override string ToString()
        {
            var sb = new StringBuilder("(");
            foreach (var instance in this._cluster)
                sb.Append($"{instance},");
            if (this._cluster.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }

        #endregion

        #region Public Methods

        public Cluster<TInstance> Clone()
        {
            return new Cluster<TInstance>(this);
        }

        public bool Contains(TInstance item)
        {
            return this._cluster.Contains(item);
        }

        public int CompareTo(Cluster<TInstance> other)
        {
            // compares by count first, then by string representation of the elements
            if (other == null) return -1;
            var countCompare = this._cluster.Length.CompareTo(other._cluster.Length);
            return countCompare == 0 ? string.CompareOrdinal(this.ToString(), other.ToString()) : countCompare;
        }

        public IEnumerator<TInstance> GetEnumerator()
        {
            return ((IEnumerable<TInstance>) this._cluster).GetEnumerator();
        }

        public bool Equals(Cluster<TInstance> other)
        {
            return !ReferenceEquals(null, other) &&
                   (ReferenceEquals(this, other) || new HashSet<TInstance>(this._cluster).SetEquals(other));
        }

        #endregion

        #region Private & Protected Methods

        private int ProduceHashCode()
        {
            unchecked
            {
                var hash = 0;
                foreach (var instance in this) hash += instance.GetHashCode();
                return 31 * hash + this.Count.GetHashCode();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}