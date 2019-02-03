using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
using LiteForum_UI.Services;
using LiteForum_UI.Models;
using System;

namespace LiteForum_UI.Shared {
  public class AlertBase : BlazorComponent, IDisposable {
    public List<AlertMessage> alerts;

    [Inject]
    protected IAlertService alertService { get; set; }
    protected override void OnInit()
    {
      this.alertService.AddAlertReceivedHandler(this.UpdateAlerts);
      this.alerts = new List<AlertMessage>();
    }

    public void UpdateAlerts(object sender, AlertMessage e) {
      if(e != null) {
        this.alerts.Add(e);
      } else {
        // route has changed removed non persisted alerts
        this.alerts.RemoveAll(t => !t.PersistOnRouteChange);
      }
      this.StateHasChanged(); // triggers update of component props
    }

    public void RemoveAlert(AlertMessage alert) => alerts.Remove(alert);

    public void Dispose() {
      this.alertService.RemoveAlertReceivedHandler(this.UpdateAlerts);
    }
  }
}
