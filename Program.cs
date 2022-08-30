using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace HackerRank
{
   class Program
   {
      static void Main(string[] args)
      {
         // Target: Getting both submissions to O(n)

         // ANZ first problem (analysis: 21 minutes, implementation: 9 minutes)
         Console.WriteLine("ANZ First Problem");
         Console.WriteLine();

         var result1 = string.Empty;

         // expect YES         
         result1 = tracesAreSimilar(
            3,
            new List<int> { 10, 10, 20, 30, 10 },
            new List<int> { 10, 20, 30, 40, 50 });
         Console.WriteLine(result1);

         // expect NO
         result1 = tracesAreSimilar(
            3,
            new List<int> { 10, 10, 20, 30, 10 },
            new List<int> { 10, 50, 50, 50, 50 });
         Console.WriteLine(result1);

         // expect NO
         result1 = tracesAreSimilar(
            1,
            new List<int> { 10, 10, 20, 30, 10 },
            new List<int> { 10, 20, 30, 40, 50 });
         Console.WriteLine(result1);

         // ANZ second problem (analysis: 6 minutes, implementation: 56 minutes)
         Console.WriteLine("ANZ Second Problem");
         Console.WriteLine();

         var result2 = 0;

         // expect 3: [4 4 2 2], [4 2], [2 4]
         result2 = countingUniversalSubarrays(new List<int> { 4, 4, 2, 2, 4 });
         Console.WriteLine(result2);

         // expect 4: [4 2], [2 4], [4 2], [2 4]
         result2 = countingUniversalSubarrays(new List<int> { 4, 2, 4, 2, 4 });
         Console.WriteLine(result2);

         // expect 0: []
         result2 = countingUniversalSubarrays(new List<int> { 2, 2, 2, 2, 2 });
         Console.WriteLine(result2);

         // expect 0: []
         result2 = countingUniversalSubarrays(new List<int> { });
         Console.WriteLine(result2);

         // expect 1: [2 4]
         result2 = countingUniversalSubarrays(new List<int> { 2, 2, 2, 2, 4 });
         Console.WriteLine(result2);

         // expect 1: [4 2]
         result2 = countingUniversalSubarrays(new List<int> { 4, 2, 2, 2, 2 });
         Console.WriteLine(result2);

         // expect 2: [4 2], [2, 4]
         result2 = countingUniversalSubarrays(new List<int> { 4, 2, 4 });
         Console.WriteLine(result2);

         // let's try this with a million records of [2 4] repeating, should yield a result of 999999
         Console.WriteLine("\nMillion record time test, should have a result of 999999\n");
         var queue = new Queue<int>();
         for (var i = 0; i < 1000000; ++i)
         {
            if (i % 2 == 0)
               queue.Enqueue(2);
            else
               queue.Enqueue(4);
         }
         var timer = new Stopwatch();
         timer.Start();
         result2 = countingUniversalSubarrays(queue.ToList());
         timer.Stop();
         Console.WriteLine(string.Format("Result {0} after time (ms): {1}", result2, timer.ElapsedMilliseconds));

         // try this with a million of halves
         Console.WriteLine("\nMillion record time test, should have a result of 2\n");
         queue = new Queue<int>();
         for (var i = 0; i < 1000000; ++i)
         {
            if (i < 500000)
               queue.Enqueue(2);
            else
               queue.Enqueue(4);
         }
         timer = new Stopwatch();
         timer.Start();
         result2 = countingUniversalSubarrays(queue.ToList());
         timer.Stop();
         Console.WriteLine(string.Format("Result {0} after time (ms): {1}", result2, timer.ElapsedMilliseconds));
      }

      public static string tracesAreSimilar(int k, List<int> s, List<int> t)
      {
         if (s.Count() != t.Count())
            return "NO";

         var dict = new Dictionary<int, int>();

         // add s
         foreach (var n in s)
         {
            if (!dict.ContainsKey(n))
               dict[n] = 0;

            ++dict[n];
         }

         // subtract t
         foreach (var n in t)
         {
            if (!dict.ContainsKey(n))
               dict[n] = 0;

            --dict[n];
         }

         // check for values larger than k
         foreach (var key in dict.Keys)
         {
            if (Math.Abs(dict[key]) > k)
               return "NO";
         }

         return "YES";
      }

      public static int countingUniversalSubarrays(List<int> arr)
      {
         if (shortCircuitCount(arr, 2))
            return 0;

         var distinct = arr.Distinct();

         if (shortCircuitCount(distinct, 1))
            return 0;

         var n1 = distinct.ElementAt(0);
         var n2 = distinct.ElementAt(1);

         var stacks = new List<Stack<int>>();

         stacks.AddRange(getStacks(arr, n1));
         stacks.AddRange(getStacks(arr, n2));

         var count = 0;
         stacks.ForEach(x => count += countUniversalSets(x));

         return count;
      }

      // short circuit count
      private static bool shortCircuitCount(IEnumerable<int> arr, int idx)
      {
         return arr.ElementAtOrDefault(idx) == default;
      }

      private static int countUniversalSets(Stack<int> stack)
      {
         // only possible for result to be 1 or 2, because:

         // odd sets:
         // [x x y] possible [x y y] impossible
         // if y has a single occurence, then always 1 count: [x y]
         // if y has multiple occurence, then always 2 count: [xxxx yyyy]

         // even sets:
         // [x y] always 1
         // [xxxx yyyy] always 2: [x y] and [xxxx yyyy]

         // always assume odd sets, it doesn't matter in the end

         // pop one then check against top of stack, if they are equal it means multiples of y
         if (stack.Pop() == stack.Peek())
            return 2;
         return 1;
      }

      private static List<Stack<int>> getStacks(List<int> arr, int n)
      {
         var nLength = 0;
         var stacks = new List<Stack<int>>();
         var stack = new Stack<int>();

         foreach (var item in arr)
         {
            // skip reads if it isn't the "n" we're looking for
            if (stack.Count() == 0 && item != n)
            {
               continue;
            }

            // if has switched to non-"n" and reading n again then abort
            // i.e. 4 4 2 4 (stack state = [4 4 2] abort because 4 is next element
            if (nLength > 0 && item == n)
            {
               nLength = 0;
               stacks.Add(stack);
               stack = new Stack<int>();
            }
            else if (item != n)
            {
               // first time reading a non-"n", save the length of stack which has just "n"
               if (nLength == 0)
                  nLength = stack.Count();
            }

            stack.Push(item);

            // desired length have been achieved, reset state
            if (nLength > 0 && stack.Count() == nLength * 2)
            {
               nLength = 0;
               stacks.Add(stack);
               stack = new Stack<int>();
            }
         }

         // reached the end of array, if valid array then add it to return
         if (nLength > 0)
            stacks.Add(stack);

         return stacks;
      }

   }
}
