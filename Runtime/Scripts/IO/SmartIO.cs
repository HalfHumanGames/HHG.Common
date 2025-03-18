//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace HHG.Common.Runtime
//{
//    public class WriteJob
//	{
//		public string FileName;
//		public byte[] Data;
//		public WriteJobType Type;
//	}

//	public enum WriteJobType
//	{
//		Clear,
//		Write
//	}

//	public abstract class SmartIO
//	{

//		protected const float writeDelay = 15f;
//		protected const float checkDelay = 1f;
//		protected static float lastWrite; // Here so shared across subclasses

//		private static bool stopTimer;

//		protected static event Action CheckJobsTimerElapsed;

//		static SmartIO()
//		{
//			CheckJobsTimer();
//		}

//		private async static void CheckJobsTimer()
//		{
//			int delayMs = (int)(checkDelay * 1000);
//			while (true && !stopTimer)
//			{
//				CheckJobsTimerElapsed?.Invoke();
//				await Task.Delay(delayMs);
//			}
//		}

//		public static bool CanWrite()
//		{
//			return Time.time - lastWrite > writeDelay;
//		}

//		public static void StopTimer()
//		{
//			stopTimer = true;
//		}
//	}

//	public class SmartIO<TIO> : SmartIO, IIO where TIO : IIO
//	{

//		private TIO diskIO;
//		private MemIO memIO = new MemIO();
//		private List<WriteJob> jobs = new List<WriteJob>();

//		public SmartIO(TIO io)
//		{
//			diskIO = io;
//			CheckJobsTimerElapsed += OnCheckJobsTimerElapsed;
//		}

//		private void OnCheckJobsTimerElapsed()
//		{
//			TryPerformNextJob();
//		}

//		private void TryPerformNextJob()
//		{
//			if (jobs.Count > 0 && CanWrite())
//			{
//				WriteJob job = jobs[0];
//				jobs.RemoveAt(0);
//				PerformJob(job);
//			}
//		}

//		private void PerformJob(WriteJob job)
//		{
//			lastWrite = Time.time;
//			switch (job.Type)
//			{
//				case WriteJobType.Clear:
//					diskIO.Delete(job.FileName);
//					memIO.Delete(job.FileName);
//					break;
//				case WriteJobType.Write:
//					diskIO.WriteAllBytes(job.FileName, job.Data);
//					memIO.WriteAllBytes(job.FileName, job.Data);
//					break;
//			}
//		}

//		private bool HasJob(string fileName)
//		{
//			return jobs.Any(job => job.FileName == fileName);
//		}

//		private void RemoveJob(string fileName)
//		{
//			jobs.RemoveAll(job => job.FileName == fileName);
//		}

//		public void Delete(string fileName)
//		{
//			RemoveJob(fileName);
//			if (CanWrite())
//			{
//				lastWrite = Time.time;
//				diskIO.Delete(fileName);
//			}
//			else
//			{
//				jobs.Add(new WriteJob
//				{
//					FileName = fileName,
//					Data = default,
//					Type = WriteJobType.Clear
//				});
//			}
//			memIO.Delete(fileName);
//		}

//		public bool Exists(string fileName)
//		{
//			if (HasJob(fileName))
//			{
//				return memIO.Exists(fileName);
//			}
//			return diskIO.Exists(fileName);
//		}

//		public byte[] ReadAllBytes(string fileName)
//		{
//			if (HasJob(fileName))
//			{
//				return memIO.ReadAllBytes(fileName);
//			}
//			return diskIO.ReadAllBytes(fileName);
//		}

//		public void WriteAllBytes(string fileName, byte[] bytes)
//		{
//			RemoveJob(fileName);
//			if (CanWrite())
//			{
//				lastWrite = Time.time;
//				diskIO.WriteAllBytes(fileName, bytes);
//			}
//			else
//			{
//				jobs.Add(new WriteJob
//				{
//					FileName = fileName,
//					Data = bytes,
//					Type = WriteJobType.Write
//				});
//			}
//			memIO.WriteAllBytes(fileName, bytes);
//		}

//		public void OnBeforeClose()
//		{
//			CheckJobsTimerElapsed -= OnCheckJobsTimerElapsed;
//			StopTimer(); // Stop timer so doesn't keep checking jobs
//			if (jobs.Count > 0)
//			{
//				WriteJob job = jobs[0];
//				jobs.RemoveAt(0);
//				PerformJob(job);
//			}
//			diskIO.OnBeforeClose();
//			memIO.OnBeforeClose();
//		}

//		public void OnClose()
//		{
//			diskIO.OnClose();
//			memIO.OnClose();
//		}
//	}
//}