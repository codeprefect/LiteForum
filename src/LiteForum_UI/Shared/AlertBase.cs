using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
using LiteForum_UI.Services;
using LiteForum_UI.Models;
using System;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;

namespace LiteForum_UI.Shared {
  public class AlertBase : BlazorComponent, IDisposable {
    public List<AlertMessage> Alerts;

    [Inject]
    protected IAlertService alertService { get; set; }

    [Inject]
    private ILogger<AlertBase> _logger { get; set; }

    protected override void OnInit()
    {
      alertService.AddAlertReceivedHandler(UpdateAlerts);
      Alerts = new List<AlertMessage>();
    }

    public void UpdateAlerts(object sender, AlertMessage e) {
      if(e != null) {
        Alerts.Add(e);
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
