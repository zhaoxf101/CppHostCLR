#pragma region Includes and Imports
#include <windows.h>
#include<stdio.h> 
#include <metahost.h>
#pragma comment(lib, "mscoree.lib")


#import "mscorlib.tlb" raw_interfaces_only				\
	high_property_prefixes("_get","_put","_putref")		\
	rename("ReportEvent", "InteropServices_ReportEvent")
using namespace mscorlib;
#pragma endregion
namespace ManageCodeInvoker
{
	 class MyClrHost
	 {
	 public:
			static __declspec(dllexport) void ExcuteManageCode(PCWSTR pszVersion,PCWSTR pszAssemblyName,  PCWSTR pszClassName,PCWSTR pszMethodName,PCWSTR argument);
			static __declspec(dllexport) void Test();
			
	 };
}
