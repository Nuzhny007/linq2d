﻿using System.Linq.Expressions;

namespace Linq2d
{
    internal abstract class ArrayQuery3<R1, R2, R3>: ArrayQueryBase
    {
        protected ArrayQuery3(ArraySource source, LambdaExpression kernel, R1 initValue1) : base(source, kernel, initValue1) { }

        protected ArrayQuery3(IArrayQuery sources, LambdaExpression kernel, R1 initValue1) : base(sources, kernel, initValue1) { }
        protected ArrayQuery3(IArrayQueryRecurrentHalf sources, LambdaExpression kernel, R2 initValue2) : base((IArrayQuery)sources, kernel)
            => ResultReplacements.Add(initValue2);

        protected ArrayQuery3(ArraySource source, LambdaExpression kernel) : base(source, kernel) { }
        protected ArrayQuery3(IArrayQuery sources, LambdaExpression kernel) : base(sources, kernel) { }
        protected ArrayQuery3(ArraySource left, ArraySource right, LambdaExpression kernel) : base(left, right, kernel) { }
        protected ArrayQuery3(IArrayQuery sources, ArraySource right, LambdaExpression kernel) : base(sources, right, kernel) { }

        protected (R1[,], R2[,], R3[,])? _result;
        protected abstract (R1[,], R2[,], R3[,]) GetResult();
        public (R1[,], R2[,], R3[,]) ToArrays()
        {
            if (!_result.HasValue)
                _result = GetResult();
            return _result.Value;
        }
    }
}
