// ReSharper disable UnusedMember.Global

using System.Management;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Kernel32;

namespace iSystem;

/// <summary>
///     The SysHelper class.
/// </summary>
public static class SystemHelper
{
	public static int ExcelColumnNameToNumber(string columnName)
	{
		if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException(nameof(columnName));

		columnName = columnName.ToUpperInvariant();

		var sum = 0;

		foreach (var t in columnName)
		{
			sum *= 26;
			sum += t - 'A' + 1;
		}

		return sum;
	}

	public static string ExcelColumnNumberToName(int columnNumber)
	{
		var columnName = string.Empty;

		if (columnNumber > 16384) return columnName;

		while (columnNumber > 0)
		{
			var modulo = (columnNumber - 1) % 26;
			columnName = Convert.ToChar('A' + modulo) + columnName;
			columnNumber = (columnNumber - modulo) / 26;
		}

		return columnName;
	}

	public static string? GetProcessorId()
	{
		var mos = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
		var moList = mos.Get();

		if (moList.Count == 0)
			return string.Empty;

		var cpuId = string.Empty;
		foreach (var mbo in moList)
		{
			var o = mbo["ProcessorID"];
			if (o is null) continue;

			cpuId = o.ToString();
		}

		return cpuId;
	}

	public static string NotifyDevicePathName(Message message, HANDLE notificationHandle)
	{
		if (message.LParam == IntPtr.Zero) goto NotifyDevicePathNameExit;
		if (notificationHandle.IsNull) goto NotifyDevicePathNameExit;

		var devBroadcastHeader =
			(DEV_BROADCAST_HDR)(Marshal.PtrToStructure(message.LParam, typeof(DEV_BROADCAST_HDR)) ??
			                    new DEV_BROADCAST_HDR());
		if (DBT_DEVTYPE.DBT_DEVTYP_DEVICEINTERFACE != devBroadcastHeader.dbch_devicetype)
			goto NotifyDevicePathNameExit;

		var devBroadcastDeviceInterface = (DEV_BROADCAST_DEVICEINTERFACE_EX)(Marshal.PtrToStructure(message.LParam,
			typeof(DEV_BROADCAST_DEVICEINTERFACE_EX)) ?? new DEV_BROADCAST_DEVICEINTERFACE_EX());

		return devBroadcastDeviceInterface.dbcc_name ?? string.Empty;

		NotifyDevicePathNameExit:
		return string.Empty;
	}

	public static SafeHDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient, Guid guid)
	{
		if (hRecipient.IsNull) goto RegisterDeviceNotificationExit;
		if (guid == Guid.Empty) goto RegisterDeviceNotificationExit;

		// Place this statement in the _Load event for the form that will receive device-change messages.
		var devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();

		var size = Convert.ToUInt32(Marshal.SizeOf(devBroadcastDeviceInterface));
		devBroadcastDeviceInterface.dbcc_size = size;
		devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYPE.DBT_DEVTYP_DEVICEINTERFACE;
		devBroadcastDeviceInterface.dbcc_reserved = 0;
		devBroadcastDeviceInterface.dbcc_classguid = guid;

		var devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(Convert.ToInt32(size));
		// Copy the structure into the buffer.
		Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);
		var notificationHandle = User32.RegisterDeviceNotification(hRecipient, devBroadcastDeviceInterfaceBuffer,
			DEVICE_NOTIFY.DEVICE_NOTIFY_WINDOW_HANDLE);

		// Free the buffer that holds information about the request.
		Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer);

		return notificationHandle;

		RegisterDeviceNotificationExit:
		return new SafeHDEVNOTIFY(IntPtr.Zero);
	}

	public static bool ReleaseConsole()
	{
		return FreeConsole();
	}

	public static void ReleaseFormCapture(Form form)
	{
		ReleaseCapture();
		SendMessage(new HWND(form.Handle), WindowMessage.WM_NCLBUTTONDOWN, HitTestValues.HTCAPTION);
	}

	public static bool SafeParse(this string source, bool defaultValue)
	{
		return bool.TryParse(source, out _) ? bool.Parse(source) : defaultValue;
	}

	public static byte SafeParse(this string source, byte defaultValue)
	{
		return byte.TryParse(source, out _) ? byte.Parse(source) : defaultValue;
	}

	public static sbyte SafeParse(this string source, sbyte defaultValue)
	{
		return sbyte.TryParse(source, out _) ? sbyte.Parse(source) : defaultValue;
	}

	public static decimal SafeParse(this string source, decimal defaultValue)
	{
		return decimal.TryParse(source, out _) ? decimal.Parse(source) : defaultValue;
	}

	public static double SafeParse(this string source, double defaultValue)
	{
		return double.TryParse(source, out _) ? double.Parse(source) : defaultValue;
	}

	public static int SafeParse(this string source, int defaultValue)
	{
		return int.TryParse(source, out _) ? int.Parse(source) : defaultValue;
	}

	public static float SafeParse(this string source, float defaultValue)
	{
		return float.TryParse(source, out _) ? float.Parse(source) : defaultValue;
	}

	public static uint SafeParse(string source, uint defaultValue)
	{
		return uint.TryParse(source, out _) ? uint.Parse(source) : defaultValue;
	}

	public static nint SafeParse(this string source, nint defaultValue)
	{
		return nint.TryParse(source, out _) ? nint.Parse(source) : defaultValue;
	}

	public static nuint SafeParse(this string source, nuint defaultValue)
	{
		return nuint.TryParse(source, out _) ? nuint.Parse(source) : defaultValue;
	}

	public static long SafeParse(this string source, long defaultValue)
	{
		return long.TryParse(source, out _) ? long.Parse(source) : defaultValue;
	}

	public static ulong SafeParse(this string source, ulong defaultValue)
	{
		return ulong.TryParse(source, out _) ? ulong.Parse(source) : defaultValue;
	}

	public static short SafeParse(this string source, short defaultValue)
	{
		return short.TryParse(source, out _) ? short.Parse(source) : defaultValue;
	}

	public static ushort SafeParse(this string source, ushort defaultValue)
	{
		return ushort.TryParse(source, out _) ? ushort.Parse(source) : defaultValue;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct DEV_BROADCAST_DEVICEINTERFACE_EX
	{
		/// <summary>
		///     The size of this structure, in bytes. This is the size of the members plus the actual length of the
		///     <c>dbcc_name</c>
		///     string
		///     (the null character is accounted for by the declaration of <c>dbcc_name</c> as a one-character array.)
		/// </summary>
		public readonly uint dbcc_size;

		/// <summary>Set to <c>DBT_DEVTYP_DEVICEINTERFACE</c>.</summary>
		public readonly DBT_DEVTYPE dbcc_devicetype;

		/// <summary>Reserved; do not use.</summary>
		public readonly uint dbcc_reserved;

		/// <summary>The GUID for the interface device class.</summary>
		public readonly Guid dbcc_classguid;

		/// <summary>
		///     <para>A null-terminated string that specifies the name of the device.</para>
		///     <para>
		///         When this structure is returned to a window through the WM_DEVICECHANGE message, the <c>dbcc_name</c> string is
		///         converted to
		///         ANSI as appropriate. Services always receive a Unicode string, whether they call
		///         <c>RegisterDeviceNotificationW</c>
		///         or <c>RegisterDeviceNotificationA</c>.
		///     </para>
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public readonly string dbcc_name;
	}
}