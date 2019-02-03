namespace LiteForum_UI.Models {
  public static class ModelExtensions {
    public static string GetAlertTypeClass(this AlertMessage alert) {
      return alert.IsSuccess() ? "alert-success"
        : (alert.IsWarning() ? "alert-warning"
        : (alert.IsError() ? "alert-danger" : ""));
    }
  }
}
