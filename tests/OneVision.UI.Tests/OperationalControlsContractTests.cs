using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class OperationalControlsContractTests
{
    [Fact]
    public void Process_stages_expose_domain_neutral_execution_metadata_and_accessible_label()
    {
        var parameters = ParametersOf<OVProcessStages>();
        Assert.Contains(nameof(OVProcessStages.Stages), parameters);
        Assert.Contains(nameof(OVProcessStages.Loading), parameters);
        Assert.Contains(nameof(OVProcessStages.ErrorMessage), parameters);
        Assert.Contains(nameof(OVProcessStages.StageAction), parameters);
        Assert.Contains(nameof(OVProcessStages.AriaLabel), parameters);

        Assert.Equal(
            new[] { "Pending", "Running", "Complete", "Failed", "Skipped", "Warning" },
            Enum.GetNames<OVProcessStageState>());
    }

    [Fact]
    public void Notification_contract_has_four_semantic_tones_and_host_controls_bounded_delivery()
    {
        Assert.Equal(
            new[] { "Success", "Information", "Warning", "Error" },
            Enum.GetNames<OVNotificationTone>());

        var host = ParametersOf<OVNotificationHost>();
        Assert.Contains(nameof(OVNotificationHost.MaximumVisible), host);
        Assert.Contains(nameof(OVNotificationHost.DefaultTimeout), host);
        Assert.Contains(nameof(OVNotificationHost.MinimumTimeout), host);
        Assert.Contains(nameof(OVNotificationHost.MaximumTimeout), host);
    }

    [Fact]
    public void Notification_service_raises_the_exact_application_request()
    {
        var service = new OVNotificationService();
        OVNotificationEntry? raised = null;
        service.NotificationRaised += entry => raised = entry;
        var request = new OVNotificationRequest(OVNotificationTone.Warning, "Message", "Title", Persistent: true, ReplacementKey: "job");

        var id = service.Show(request);

        Assert.NotNull(raised);
        Assert.Equal(id, raised!.Id);
        Assert.Same(request, raised.Request);
        Assert.True(raised.Request.Persistent);
        Assert.Equal("job", raised.Request.ReplacementKey);
    }

    [Fact]
    public void File_upload_keeps_application_validation_and_processing_outside_the_control()
    {
        var parameters = ParametersOf<OVFileUpload>();
        Assert.Contains(nameof(OVFileUpload.SelectedFiles), parameters);
        Assert.Contains(nameof(OVFileUpload.SelectedFilesChanged), parameters);
        Assert.Contains(nameof(OVFileUpload.ValidateFile), parameters);
        Assert.Contains(nameof(OVFileUpload.UploadProgress), parameters);
        Assert.Equal(typeof(Func<IBrowserFile, string?>), typeof(OVFileUpload).GetProperty(nameof(OVFileUpload.ValidateFile))!.PropertyType);
    }

    [Fact]
    public void Stepper_and_process_stages_are_separate_semantic_contracts()
    {
        Assert.Contains(nameof(OVStepper.StepSelected), ParametersOf<OVStepper>());
        Assert.Contains(OVStepState.Current, Enum.GetValues<OVStepState>());
        Assert.Contains(OVProcessStageState.Running, Enum.GetValues<OVProcessStageState>());
        Assert.NotEqual(typeof(OVStep), typeof(OVProcessStage));
    }

    [Fact]
    public void Operational_detail_exposes_readonly_status_composition_and_overlay_action_slots()
    {
        var parameters = ParametersOf<OVOperationalDetail>();
        var required = new[]
        {
            nameof(OVOperationalDetail.Status), nameof(OVOperationalDetail.EnabledControl), nameof(OVOperationalDetail.PrimaryAction),
            nameof(OVOperationalDetail.EditAction), nameof(OVOperationalDetail.OverflowActions), nameof(OVOperationalDetail.HealthSummary),
            nameof(OVOperationalDetail.Metadata), nameof(OVOperationalDetail.LatestEvent), nameof(OVOperationalDetail.ProcessStages),
            nameof(OVOperationalDetail.RecentActivity), nameof(OVOperationalDetail.Alerts)
        };
        Assert.All(required, name => Assert.Contains(name, parameters));
    }

    [Fact]
    public void Supporting_operational_controls_expose_accessible_or_semantic_inputs()
    {
        Assert.Contains(nameof(OVTimeline.AriaLabel), ParametersOf<OVTimeline>());
        Assert.Contains(nameof(OVList.AriaLabel), ParametersOf<OVList>());
        Assert.Contains(nameof(OVStepper.AriaLabel), ParametersOf<OVStepper>());
        Assert.Contains(nameof(OVBadge.Label), ParametersOf<OVBadge>());
        Assert.Contains(nameof(OVDivider.Orientation), ParametersOf<OVDivider>());
        Assert.Contains(nameof(OVDateTime.TimeZone), ParametersOf<OVDateTime>());
        Assert.Contains(nameof(OVSkeleton.Animate), ParametersOf<OVSkeleton>());
        Assert.Contains(nameof(OVSelectionList.SelectedKeysChanged), ParametersOf<OVSelectionList>());
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
