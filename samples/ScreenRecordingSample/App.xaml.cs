using Autofac;
using Plugin.Maui.ScreenRecording;
namespace ScreenRecordingSample;


public partial class App : Application
{
	public static Autofac.IContainer Container;
	static readonly Autofac.ContainerBuilder builder = new Autofac.ContainerBuilder();

	public App()
	{
		InitializeComponent();


		builder.RegisterType<ScreenRecordingImplementation>().As<IScreenRecording>().SingleInstance();
		Container = (Autofac.IContainer)builder.Build();

		MainPage = new AppShell();
	}
}