use agreed;
use agreed::engine::engine_server::EngineServer;
use agreed::grpc::Server as GrpcService;
use tonic::transport::Server;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
	println!("Starting validation server");

	let port = 5003;
	let addr = format!("0.0.0.0:{}", port).parse()?;

	let svc = GrpcService::new();
	Server::builder().add_service(EngineServer::new(svc)).serve(addr).await?;

	Ok(())
}
