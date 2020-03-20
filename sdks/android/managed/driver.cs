
	public class Driver
	{
		public static Process Run (string testSuite, int port = 6100)
		{
			ProcessStartInfo psi1 = new ProcessStartInfo ("adb", $"reverse tcp:{port} tcp:{port}");

			Console.WriteLine ($"Execute \"{psi1.FileName} {psi1.Arguments}\"");
			using (Process p = Process.Start (psi1)) {
				p.WaitForExit();
			}

			bool waitForLLDB = Environment.GetEnvironmentVariable("MONO_WAIT_LLDB") != null;

			ProcessStartInfo psi2 = new ProcessStartInfo ("adb", $"shell am instrument -w -e TestSuite \"{testSuite}\" -e WaitForLLDB \"{waitForLLDB}\" \"{App}\"") {
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
			};

			Console.WriteLine ($"Execute \"{psi2.FileName} {psi2.Arguments}\"");
			Process adb = Process.Start (psi2);
			adb.OutputDataReceived += (s, e) => { if (e.Data != null) Console.Out.Write (e.Data + "\n"); };
			adb.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.Write (e.Data + "\n"); };
			adb.BeginOutputReadLine ();
			adb.BeginErrorReadLine ();

			return adb;
		}
	}