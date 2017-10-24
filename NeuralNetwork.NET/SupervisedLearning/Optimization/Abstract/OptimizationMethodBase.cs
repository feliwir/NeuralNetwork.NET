﻿// ===================================
// Credits for the base of this code
// ===================================
//
// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

using System;
using System.Threading;
using NeuralNetworkNET.Helpers;

namespace NeuralNetworkNET.SupervisedLearning.Optimization.Abstract
{
    /// <summary>
    ///   Base class for optimization methods
    /// </summary>
    public abstract class OptimizationMethodBase
    {
        /// <summary>
        ///   Gets or sets a cancellation token that can be used to
        ///   stop the learning algorithm while it is running
        /// </summary>
        public CancellationToken Token { get; set; } = new CancellationToken();

        /// <summary>
        ///   Gets or sets the function to be optimized
        /// </summary>
        /// 
        /// <value>The function to be optimized</value>
        public Func<double[], double> Function { get; set; }

        private int _NumberOfVariables;

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem
        /// </summary>
        /// 
        /// <value>The number of parameters</value>
        public virtual int NumberOfVariables
        {
            get => _NumberOfVariables;
            set
            {
                _NumberOfVariables = value;
                OnNumberOfVariablesChanged(value);
            }
        }

        /// <summary>
        /// Called when the <see cref="NumberOfVariables"/> property has changed
        /// </summary>
        private void OnNumberOfVariablesChanged(int numberOfVariables)
        {
            Random random = new Random();
            if (Solution == null || Solution.Length != numberOfVariables)
            {
                Solution = new double[numberOfVariables];
                for (int i = 0; i < Solution.Length; i++)
                    Solution[i] = random.NextGaussian();
            }
        }

        private double[] _Solution;

        /// <summary>
        ///   Gets the current solution found, the values of 
        ///   the parameters which optimizes the function
        /// </summary>
        public double[] Solution
        {
            get => _Solution;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Solution));
                if (value.Length != NumberOfVariables) throw new ArgumentException(nameof(Solution), "Invalid solution size");
                _Solution = value;
            }
        }

        /// <summary>
        ///   Gets the output of the function at the current <see cref="Solution"/>
        /// </summary>
        public double Value { get; protected set; }

        /// <summary>
        ///   Finds the minimum value of a function. The solution vector
        ///   will be made available at the <see cref="Solution"/> property
        /// </summary>
        /// 
        /// <returns>
        ///   Returns <c>true</c> if the method converged to a <see cref="Solution"/>.
        ///   In this case, the found value will also be available at the <see cref="Value"/> property
        /// </returns>
        public virtual bool Minimize()
        {
            if (Function == null) throw new InvalidOperationException("function");
            bool success = Optimize();
            Value = Function(Solution);
            return success;
        }

        /// <summary>
        ///   Implements the actual optimization algorithm. This
        ///   method should try to minimize the objective function.
        /// </summary>
        /// 
        protected abstract bool Optimize();
    }
}