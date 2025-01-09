mergeInto(LibraryManager.library, {
  GetBrowserTime: function () {
      var currentTime = new Date();
      if (!currentTime) {
          console.error("Failed to get current time.");
          return "";  // nullやundefinedが返らないように空文字列を返す
      }
      var utcTime = currentTime.toISOString();
      console.log("GetBrowserTime (UTC): " + utcTime);
      return utcTime;
  }
});