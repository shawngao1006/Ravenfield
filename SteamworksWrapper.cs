using System;
using System.Collections.Generic;
using UnityEngine;
#if STEAMWORKS
using Steamworks;
#endif

// Token: 0x02000268 RID: 616
#if STEAMWORKS
public class SteamworksWrapper
{
	// original Steamworks-enabled implementation remains unchanged for builds with STEAMWORKS defined
#else
// Minimal no-op Steamworks replacements when STEAMWORKS is not defined.
namespace Steamworks
{
	public struct PublishedFileId_t
	{
		public ulong m_PublishedFileId;
		public PublishedFileId_t(ulong v) { m_PublishedFileId = v; }
		public override string ToString() { return m_PublishedFileId.ToString(); }
	}
	public struct SteamUGCDetails_t { public PublishedFileId_t m_nPublishedFileId; }
	public struct UGCQueryHandle_t { public ulong handle; public UGCQueryHandle_t(ulong v) { handle = v; } }
	public struct UGCUpdateHandle_t { public ulong handle; public UGCUpdateHandle_t(ulong v) { handle = v; } }
	public struct SteamAPICall_t { public ulong m_Handle; public SteamAPICall_t(ulong v) { m_Handle = v; } }
	public struct CSteamID { public ulong m_SteamID; public CSteamID(ulong id) { m_SteamID = id; } }
	public struct AppId_t { public uint id; public AppId_t(uint v) { id = v; } }
	public enum EResult { k_EResultOK = 1 }
	public enum EItemUpdateStatus { k_EItemUpdateStatusPreparingContent, k_EItemUpdateStatusPreparingConfig }
	public enum ERemoteStoragePublishedFileVisibility { k_ERemoteStoragePublishedFileVisibilityPrivate, k_ERemoteStoragePublishedFileVisibilityFriendsOnly, k_ERemoteStoragePublishedFileVisibilityPublic }
	public enum EUGCQuery { k_EUGCQuery_RankedByVotesUp }
	public enum EUGCMatchingUGCType { k_EUGCMatchingUGCType_Items_ReadyToUse }

	// Input handle placeholders
	public struct InputAnalogActionHandle_t { public ulong handle; public InputAnalogActionHandle_t(ulong v) { handle = v; } }
	public struct InputDigitalActionHandle_t { public ulong handle; public InputDigitalActionHandle_t(ulong v) { handle = v; } }
	public struct InputAnalogActionData_t { }
}

public class SteamworksWrapper
{
	public const uint APP_ID = 636480U;
	public const string WORKSHOP_TERMS_OF_SERVICE_URL = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";
	public const string COMMUNITY_FILE_PAGE_URL = "steam://url/CommunityFilePage/";

	public SteamInputWrapper input = new SteamInputWrapper();
	public bool isInitialized = false;
	public Steamworks.EResult lastResult = 0;
	public string errorMessage = null;
	public Steamworks.PublishedFileId_t[] subscribedItems = null;
	public SteamworksWrapper.WorkshopItem currentItem = null;

	public delegate void DelOnStateChanged();
	public delegate void DelOnCreateItemDone(bool ok, Steamworks.PublishedFileId_t itemId);
	public delegate void DelOnSubmitItemDone(bool ok);
	public delegate void DelOnUGCQueryDone(bool ok, SteamworksWrapper.UGCQueryResult[] details);
	public delegate void DelOnItemInstalled(Steamworks.PublishedFileId_t itemId);
	public delegate void DelOnRemotePublishedFileSubscribed(Steamworks.PublishedFileId_t itemId);
	public delegate void DelOnRemotePublishedFileUnsubscribed(Steamworks.PublishedFileId_t itemId);

	public DelOnStateChanged OnStateChanged;
	public DelOnCreateItemDone OnCreateItemDone;
	public DelOnSubmitItemDone OnSubmitItemDone;
	public DelOnItemInstalled OnItemInstalled;
	public DelOnRemotePublishedFileSubscribed OnRemotePublishedFileSubscribed;
	public DelOnRemotePublishedFileUnsubscribed OnRemotePublishedFileUnsubscribed;

	public SteamworksWrapper() { }

	private void SignalStateChanged() { if (OnStateChanged != null) OnStateChanged(); }

	public bool Initialize() { isInitialized = false; SignalStateChanged(); return false; }
	public void Update() { }
	public void Shutdown() { isInitialized = false; SignalStateChanged(); }
	public string Username() { return "LocalUser"; }
	public bool HasCurrentItem() { return currentItem != null; }
	public void SetCurrentItem(SteamworksWrapper.WorkshopItem item) { currentItem = item; SignalStateChanged(); }
	public void SetCurrentItemId(ulong id) { SetCurrentItem(new WorkshopItem(new Steamworks.PublishedFileId_t(id))); }
	public void DropCurrentItem() { currentItem = null; }

	public void CreateWorkshopItem() { }
	public void SubmitCurrentItem(string contentPath, string changeNote) { }
	public bool IsUploadingItem() { return false; }
	public bool IsPreparingContentUpload() { return false; }
	public float GetUploadProgress() { return 0f; }

	public Steamworks.UGCQueryHandle_t CreateUGCQuery(DelOnUGCQueryDone ManagedCallback) { return new Steamworks.UGCQueryHandle_t(0UL); }
	public void AddRequiredTagUGCQuery(Steamworks.UGCQueryHandle_t query, string tag) { }
	public void SendUGCQueryRequest(Steamworks.UGCQueryHandle_t query) { }
	public void QuickQueryItemInfo(Steamworks.PublishedFileId_t[] fileIds, DelOnUGCQueryDone ManagedCallback) { ManagedCallback(true, new UGCQueryResult[0]); }

	public void SubscribeToItem(Steamworks.PublishedFileId_t fileId) { }
	public uint GetItemState(Steamworks.PublishedFileId_t fileId) { return 0U; }
	public uint FetchSubscribedItems() { subscribedItems = new Steamworks.PublishedFileId_t[0]; return 0U; }
	public bool HasFetchedSubscribedItems() { return subscribedItems != null; }
	public bool IsSubscribedItemInstalled(Steamworks.PublishedFileId_t itemId) { return false; }
	public string[] GetSubscribedItemPaths() { return new string[0]; }
	public string GetSubscribedItemPath(Steamworks.PublishedFileId_t id) { return string.Empty; }

	public string GetSteamNick() { return "LocalUser"; }
	public ulong GetSteamId() { return 0UL; }

	private void SetError(string error) { this.errorMessage = error; }

	public void OpenCommunityFilePage(Steamworks.PublishedFileId_t itemId) { OpenUrl(COMMUNITY_FILE_PAGE_URL + itemId.m_PublishedFileId.ToString(), false); }
	public void OpenUrl(string url, bool inSteamOverlay = true) { Application.OpenURL(url); }

	public class WorkshopItem
	{
		public Steamworks.PublishedFileId_t itemId;
		public string title;
		public string description;
		public string previewImagePath;
		public List<string> tags;
		public WorkshopItem.Visibility visibility;
		public WorkshopItem(Steamworks.PublishedFileId_t itemId) { this.itemId = itemId; this.visibility = WorkshopItem.Visibility.NoChange; }
		public override string ToString() { return "WorkshopItem #" + itemId.m_PublishedFileId.ToString(); }
		public enum Visibility { NoChange, Private, FriendsOnly, Public }
	}

	public struct UGCQueryResult { public Steamworks.SteamUGCDetails_t details; public string previewImageURL; }
}
#endif
		{
			if (!this.IsSubscribedItemInstalled(itemId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0000D7CB File Offset: 0x0000B9CB
	public bool IsSubscribedItemInstalled(PublishedFileId_t itemId)
	{
		return (SteamUGC.GetItemState(itemId) & 4U) > 0U;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0008B63C File Offset: 0x0008983C
	public string[] GetSubscribedItemPaths()
	{
		this.CheckInitialized();
		if (!this.HasFetchedSubscribedItems())
		{
			throw new Exception("No subscribed items list has been fetched");
		}
		string[] array = new string[this.subscribedItems.Length];
		for (int i = 0; i < this.subscribedItems.Length; i++)
		{
			ulong num;
			string text;
			uint num2;
			SteamUGC.GetItemInstallInfo(this.subscribedItems[i], out num, out text, 0U, out num2);
			array[i] = text;
		}
		return array;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0008B6A8 File Offset: 0x000898A8
	public string GetSubscribedItemPath(PublishedFileId_t id)
	{
		uint cchFolderSize = 4096U;
		ulong num;
		string result;
		uint num2;
		SteamUGC.GetItemInstallInfo(id, out num, out result, cchFolderSize, out num2);
		return result;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0000D7D8 File Offset: 0x0000B9D8
	public string GetSteamNick()
	{
		return SteamFriends.GetPersonaName();
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0000D7DF File Offset: 0x0000B9DF
	public ulong GetSteamId()
	{
		return SteamUser.GetSteamID().m_SteamID;
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0000D7EB File Offset: 0x0000B9EB
	public void DropCurrentItem()
	{
		this.currentItem = null;
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x0000D7F4 File Offset: 0x0000B9F4
	private void SetError(string error)
	{
		Debug.LogError(error);
		this.errorMessage = error;
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x0000D803 File Offset: 0x0000BA03
	public void OpenCommunityFilePage(PublishedFileId_t itemId)
	{
		this.OpenUrl("steam://url/CommunityFilePage/" + itemId.m_PublishedFileId.ToString(), true);
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x0000D822 File Offset: 0x0000BA22
	public void OpenUrl(string url, bool inSteamOverlay = true)
	{
		if (inSteamOverlay)
		{
			this.CheckInitialized();
			SteamFriends.ActivateGameOverlayToWebPage(url, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
			return;
		}
		Application.OpenURL(url);
	}

	// Token: 0x0400123E RID: 4670
	public const uint APP_ID = 636480U;

	// Token: 0x0400123F RID: 4671
	public const string WORKSHOP_TERMS_OF_SERVICE_URL = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";

	// Token: 0x04001240 RID: 4672
	public const string COMMUNITY_FILE_PAGE_URL = "steam://url/CommunityFilePage/";

	// Token: 0x04001241 RID: 4673
	public SteamInputWrapper input = new SteamInputWrapper();

	// Token: 0x04001242 RID: 4674
	public bool isInitialized;

	// Token: 0x04001243 RID: 4675
	protected CallResult<CreateItemResult_t> createItemResult;

	// Token: 0x04001244 RID: 4676
	protected CallResult<SubmitItemUpdateResult_t> submitItemResult;

	// Token: 0x04001245 RID: 4677
	protected Callback<ItemInstalled_t> itemInstalled;

	// Token: 0x04001246 RID: 4678
	protected Callback<RemoteStoragePublishedFileSubscribed_t> remotePublishedFileSubscribed;

	// Token: 0x04001247 RID: 4679
	protected Callback<RemoteStoragePublishedFileUnsubscribed_t> remotePublishedFileUnsubscribed;

	// Token: 0x04001248 RID: 4680
	public SteamworksWrapper.DelOnStateChanged OnStateChanged;

	// Token: 0x04001249 RID: 4681
	public SteamworksWrapper.DelOnCreateItemDone OnCreateItemDone;

	// Token: 0x0400124A RID: 4682
	public SteamworksWrapper.DelOnSubmitItemDone OnSubmitItemDone;

	// Token: 0x0400124B RID: 4683
	public SteamworksWrapper.DelOnItemInstalled OnItemInstalled;

	// Token: 0x0400124C RID: 4684
	public SteamworksWrapper.DelOnRemotePublishedFileSubscribed OnRemotePublishedFileSubscribed;

	// Token: 0x0400124D RID: 4685
	public SteamworksWrapper.DelOnRemotePublishedFileUnsubscribed OnRemotePublishedFileUnsubscribed;

	// Token: 0x0400124E RID: 4686
	public bool needsToAcceptWorkshopLegalAgreement;

	// Token: 0x0400124F RID: 4687
	public SteamworksWrapper.WorkshopItem currentItem;

	// Token: 0x04001250 RID: 4688
	public EResult lastResult;

	// Token: 0x04001251 RID: 4689
	public string errorMessage;

	// Token: 0x04001252 RID: 4690
	public PublishedFileId_t[] subscribedItems;

	// Token: 0x04001253 RID: 4691
	private bool isUpdatingItem;

	// Token: 0x04001254 RID: 4692
	private UGCUpdateHandle_t lastUpdateHandle;

	// Token: 0x04001255 RID: 4693
	private Dictionary<UGCQueryHandle_t, CallResult<SteamUGCQueryCompleted_t>> queryUGCCallResultHandler;

	// Token: 0x02000269 RID: 617
	// (Invoke) Token: 0x060010F4 RID: 4340
	public delegate void DelOnStateChanged();

	// Token: 0x0200026A RID: 618
	// (Invoke) Token: 0x060010F8 RID: 4344
	public delegate void DelOnCreateItemDone(bool ok, PublishedFileId_t itemId);

	// Token: 0x0200026B RID: 619
	// (Invoke) Token: 0x060010FC RID: 4348
	public delegate void DelOnSubmitItemDone(bool ok);

	// Token: 0x0200026C RID: 620
	// (Invoke) Token: 0x06001100 RID: 4352
	public delegate void DelOnUGCQueryDone(bool ok, SteamworksWrapper.UGCQueryResult[] details);

	// Token: 0x0200026D RID: 621
	// (Invoke) Token: 0x06001104 RID: 4356
	public delegate void DelOnItemInstalled(PublishedFileId_t itemId);

	// Token: 0x0200026E RID: 622
	// (Invoke) Token: 0x06001108 RID: 4360
	public delegate void DelOnRemotePublishedFileSubscribed(PublishedFileId_t itemId);

	// Token: 0x0200026F RID: 623
	// (Invoke) Token: 0x0600110C RID: 4364
	public delegate void DelOnRemotePublishedFileUnsubscribed(PublishedFileId_t itemId);

	// Token: 0x02000270 RID: 624
	public class WorkshopItem
	{
		// Token: 0x0600110F RID: 4367 RVA: 0x0000D83B File Offset: 0x0000BA3B
		public WorkshopItem(PublishedFileId_t itemId)
		{
			this.itemId = itemId;
			this.visibility = SteamworksWrapper.WorkshopItem.Visibility.NoChange;
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x0008B6CC File Offset: 0x000898CC
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"WokshopItem #",
				this.itemId.m_PublishedFileId.ToString(),
				": ",
				this.title,
				", ",
				this.description,
				", "
			});
		}

		// Token: 0x04001256 RID: 4694
		public PublishedFileId_t itemId;

		// Token: 0x04001257 RID: 4695
		public string title;

		// Token: 0x04001258 RID: 4696
		public string description;

		// Token: 0x04001259 RID: 4697
		public string previewImagePath;

		// Token: 0x0400125A RID: 4698
		public List<string> tags;

		// Token: 0x0400125B RID: 4699
		public SteamworksWrapper.WorkshopItem.Visibility visibility;

		// Token: 0x02000271 RID: 625
		public enum Visibility
		{
			// Token: 0x0400125D RID: 4701
			NoChange,
			// Token: 0x0400125E RID: 4702
			Private,
			// Token: 0x0400125F RID: 4703
			FriendsOnly,
			// Token: 0x04001260 RID: 4704
			Public
		}
	}

	// Token: 0x02000272 RID: 626
	public struct UGCQueryResult
	{
		// Token: 0x04001261 RID: 4705
		public SteamUGCDetails_t details;

		// Token: 0x04001262 RID: 4706
		public string previewImageURL;
	}
}
