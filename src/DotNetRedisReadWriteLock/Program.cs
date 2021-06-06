using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RReadLockPOC
{
    class Program
    {
        

        static void Main(string[] args)
        {
            int amount = 0;
            List<Task> tasks = new List<Task>();

            foreach (var item in Enumerable.Range(1, 10))
            {
                tasks.Add(Task.Run(() =>
                {
                    ReadLock readLock = new ReadLock("lockkey1");
                    readLock.TryLock(10);
                    Console.WriteLine($"[ReadLock] get lock {DateTime.Now:yyyy MM dd hh:mm:ss:fff}");
                    Console.WriteLine($"[ReadLock] Read amount :{amount}"); 
                    readLock.UnLock();
                    Console.WriteLine($"[ReadLock] release! {DateTime.Now:yyyy MM dd hh:mm:ss:fff}");
                }));

                tasks.Add(Task.Run(() =>
                {
                    WriteLock writeLock = new WriteLock("lockkey1");
                    writeLock.TryLock(5);
                    Console.WriteLine($"[WriteLock] get lock {DateTime.Now:yyyy MM dd hh:mm:ss:fff}  {writeLock.GetLockName()}");
                    amount++;
                    writeLock.UnLock();
                    Console.WriteLine($"[WriteLock] release! {DateTime.Now:yyyy MM dd hh:mm:ss:fff}");
                }));

                tasks.Add(Task.Run(() =>
                {
                    WriteLock writeLock = new WriteLock("lockkey1");
                    writeLock.TryLock(5);
                    Console.WriteLine($"[WriteLock] get lock {DateTime.Now:yyyy MM dd hh:mm:ss:fff}  {writeLock.GetLockName()}");
                    amount++;
                    writeLock.UnLock();
                    Console.WriteLine($"[WriteLock] release! {DateTime.Now:yyyy MM dd hh:mm:ss:fff}");
                }));

            }



            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"amount:{amount}");
            Console.WriteLine("完成");

            Console.ReadKey();
        }

        /*
         *
         *KEYS：
 KEYS1 = anyLock
 
 KEYS[2] = redisson_rwlock:{anyLock}
 
 KEYS[3] = {anyLock}:UUID_01:threadId_01:rwlock_timeout
 
 KEYS[4] = {anyLock}
 
 ARGV[1] = 0
 
 ARGV[2] = UUID_01:threadId_01
         *
         */
    }
}
