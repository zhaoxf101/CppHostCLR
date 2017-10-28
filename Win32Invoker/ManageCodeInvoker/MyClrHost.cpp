#include "LoadClr.h"

 namespace ManageCodeInvoker
{

		   	void MyClrHost:: ExcuteManageCode(PCWSTR pszVersion,PCWSTR pszAssemblyPath,  PCWSTR pszClassName,PCWSTR pszMethodName,PCWSTR argument)
			{
				HRESULT hr;
				ICLRMetaHost *pMetaHost = NULL;
				ICLRRuntimeInfo *pRuntimeInfo = NULL;
				ICLRRuntimeHost *pClrRuntimeHost = NULL;
				DWORD dwLengthRet;

				hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));//创建实例
	            if(FAILED(hr))
				{
					goto Cleanup;
				}
				  hr = pMetaHost->GetRuntime(pszVersion, IID_PPV_ARGS(&pRuntimeInfo));//获取CLR信息
					if (FAILED(hr))
					{
						goto Cleanup;
					}
					    BOOL fLoadable;
					hr = pRuntimeInfo->IsLoadable(&fLoadable);
					if (FAILED(hr))
					{
						goto Cleanup;
					}

					if (!fLoadable)
					{
						goto Cleanup;
					}
					hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, //初始化ClrRuntimeHost
					IID_PPV_ARGS(&pClrRuntimeHost));
					if (FAILED(hr))
					{
						wprintf(L"ICLRRuntimeInfo::GetInterface failed w/hr 0x%08lx\n", hr);
						goto Cleanup;
					}
					    hr = pClrRuntimeHost->Start();//启动CLR
					if (FAILED(hr))
					{
						wprintf(L"CLR failed to start w/hr 0x%08lx\n", hr);
						goto Cleanup;
					}
					//执行代码
					 hr = pClrRuntimeHost->ExecuteInDefaultAppDomain(pszAssemblyPath, 
					pszClassName, pszMethodName, argument,&dwLengthRet);
					 pClrRuntimeHost->Stop();
					if (FAILED(hr))
					{
						goto Cleanup;
					}
				Cleanup:

					if (pMetaHost)
					{
						pMetaHost->Release();
						pMetaHost = NULL;
					}
					if (pRuntimeInfo)
					{
						pRuntimeInfo->Release();
						pRuntimeInfo = NULL;
					}
			}

			void MyClrHost::Test()
			{
				ManageCodeInvoker::MyClrHost::ExcuteManageCode(L"v4.0.30319",L"E:\\Message.dll",L"Message.Message",L"Show",L"HelloWord");
			}
			
 }
 