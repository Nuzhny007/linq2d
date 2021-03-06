﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.Processing2d.Slow
{
    public static class SlowArray2d
    {
        public static R[,] Select<T, R>(this T[,] source, Func<T, R> kernel)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[i, j] = kernel(source[i, j]);

            return result;
        }

        public static R[,] Select<T, R>(this IRelQueryableArray2d<T> source,Kernel<T, R> kernel)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];

            var km = KernelMeasure.Measure(kernel);

            var cell = new Cell<T>(source);

            for (cell._x = -km.xmin; cell._x < h - km.xmax; cell._x++)
                for (cell._y = -km.ymin; cell._y < w - km.ymax; cell._y++)
                    result[cell._x, cell._y] = kernel(cell);

            return (result);
        }

        public static R[,] SelectMany<T, A, R>(this T[,] source, Func<T, IQueryableArray2d<A>> secondSelector, Func<T, A, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];
            var second = secondSelector(default);
            if (second.GetLength(0) < h)
                throw new ArgumentOutOfRangeException("secondArray.height", second.GetLength(0), $"should be no less than {h}");
            if (second.GetLength(1) < w)
                throw new ArgumentOutOfRangeException("secondArray.width", second.GetLength(1), $"should be no less than {w}");


            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    result[i, j] = resultSelector(source[i, j], second[i, j]);

            return (result);
        }

        public static R[,] SelectMany<T, A, R>(this T[,] source, Func<T, IRelQueryableArray2d<A>> secondSelector, Func<T, ICell<A>, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];
            var second = secondSelector(default);
            if (second.GetLength(0) < h)
                throw new ArgumentOutOfRangeException("secondArray.height", second.GetLength(0), $"should be no less than {h}");
            if (second.GetLength(1) < w)
                throw new ArgumentOutOfRangeException("secondArray.width", second.GetLength(1), $"should be no less than {w}");

            var km = KernelMeasure.Measure((ICell<A> c) => resultSelector(default, c));

            var cell = new Cell<A>(second);

            for (cell._x = -km.xmin; cell._x < h - km.xmax; cell._x++)
                for (cell._y = -km.ymin; cell._y < w - km.ymax; cell._y++)
                    result[cell._x, cell._y] = resultSelector(source[cell._x, cell._y], cell);

            return (result);
        }

        public static R[,] SelectMany<T, R>(this T[,] source, Func<T, ArrayRecurrence<R>> secondSelector, Func<T, ICell<R>, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];

            var km = KernelMeasure.Measure((ICell<R> c) => resultSelector(default, c));

            var resCell = new Cell<R>(result.Wrap(Bounds.Skip));

            for (resCell._x = -km.xmin; resCell._x < h - km.xmax; resCell._x++)
                for (resCell._y = -km.ymin; resCell._y < w - km.ymax; resCell._y++)
                    result[resCell._x, resCell._y] = resultSelector(source[resCell._x, resCell._y], resCell);

            return (result);
        }

        public static R[,] SelectMany<T, A, R>(this IRelQueryableArray2d<T> source, Func<ICell<T>, A[,]> secondSelector, Func<ICell<T>, A, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];
            var second = secondSelector(null);
            if (second.GetLength(0) < h)
                throw new ArgumentOutOfRangeException("secondArray.height", second.GetLength(0), $"should be no less than {h}");
            if (second.GetLength(1) < w)
                throw new ArgumentOutOfRangeException("secondArray.width", second.GetLength(1), $"should be no less than {w}");

            var km = KernelMeasure.Measure((ICell<T> c) => resultSelector(c, default));

            var cell = new Cell<T>(source);

            for (cell._x = -km.xmin; cell._x < h - km.xmax; cell._x++)
                for (cell._y = -km.ymin; cell._y < w - km.ymax; cell._y++)
                    result[cell._x, cell._y] = resultSelector(cell, second[cell._x, cell._y]);

            return (result);
        }

        public static R[,] SelectMany<T, A, R>(this IRelQueryableArray2d<T> source, Func<ICell<T>, IRelQueryableArray2d<A>> secondSelector, Func<ICell<T>, ICell<A>, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];
            var second = secondSelector(null);
            if (second.GetLength(0) < h)
                throw new ArgumentOutOfRangeException("secondArray.height", second.GetLength(0), $"should be no less than {h}");
            if (second.GetLength(1) < w)
                throw new ArgumentOutOfRangeException("secondArray.width", second.GetLength(1), $"should be no less than {w}");

            var m = new KernelMeasure();
            var km1 = new KernelMeasure<T>(m);
            var km2 = new KernelMeasure<A>(m);
            resultSelector(km1, km2);
            var cell1 = new Cell<T>(source);
            var cell2 = new Cell<A>(second);

            for (cell1._x = -m.xmin; cell1._x < h - m.xmax; cell1._x++)
            {
                cell2._x = cell1._x;
                for (cell1._y = -m.ymin; cell1._y < w - m.ymax; cell1._y++)
                {
                    cell2._y = cell1._y;
                    result[cell1._x, cell1._y] = resultSelector(cell1, cell2);
                }
            }
            return (result);
        }

        public static R[,] SelectMany<T, R>(this IRelQueryableArray2d<T> source, Func<ICell<T>, ArrayRecurrence<R>> secondSelector, Func<ICell<T>, ICell<R>, R> resultSelector)
        {
            var h = source.GetLength(0);
            var w = source.GetLength(1);
            var result = new R[h, w];

            var m = new KernelMeasure();
            var km1 = new KernelMeasure<T>(m);
            var km2 = new KernelMeasure<R>(m);
            resultSelector(km1, km2);

            var cell = new Cell<T>(source);
            var resCell = new Cell<R>(result.Wrap(Bounds.Skip));

            for (resCell._x = -m.xmin; resCell._x < h - m.xmax; resCell._x++)
            {
                cell._x = resCell._x;
                for (resCell._y = -m.ymin; resCell._y < w - m.ymax; resCell._y++)
                {
                    cell._y = resCell._y;
                    result[resCell._x, resCell._y] = resultSelector(cell, resCell);
                }
            }

            return (result);
        }
    }
}
