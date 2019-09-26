using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LiteForum_UI.Services;
using LiteForum_UI.Models;
using System;
using Microsoft.JSInterop;

namespace LiteForum_UI.Shared {
  public class AlertBase : ComponentBase, IDisposable {
    public List<AlertMessage> Alerts;

    [Inject]
    protected IAlertService alertService { get; set; }

    protected override void OnInitialized()
    {
      alertService.AddAlertReceivedHandler(UpdateAlerts);
      Alerts = new List<AlertMessage>();
    }

    public void UpdateAlerts(object sender, AlertMessage e) {
      if(e != null) {
        Alerts.Insert(0, e);
        RemoveOnExpiry(e);
      } else {
          // route has changed removed non persisted alerts
        Alerts.RemoveAll(t => !t.PersistOnRouteChange);
      }
      StateHasChanged(); // triggers update of component props
    }

    public void RemoveAlert(AlertMessage alert) => Alerts.Remove(alert);

    public void Dispose() {
      alertService.RemoveAlertReceivedHandler(UpdateAlerts);
    }

    public void RemoveOnExpiry(AlertMessage alert) {
      int persistInMinutes = alert.IsSuccess() ? 1
        : (alert.IsWarning() ? 3
        : (alert.IsError() ? 10 : 0));
      Task.Delay(persistInMinutes * 1000 * 60).ContinueWith(_ => {
        RemoveAlert(alert);
        StateHasChanged(); // triggers update of component props
      });
    }
  }
}
