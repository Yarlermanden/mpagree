use engine;
use engine::grpc::Server as GrpcService;
use engine::validator::validator_server::ValidatorServer;
use tonic::transport::Server;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
	println!("Starting validation server");

	let port = 5003;
	let addr = format!("0.0.0.0:{}", port).parse()?;

	let svc = GrpcService::new();
	Server::builder().add_service(ValidatorServer::new(svc)).serve(addr).await?;

	Ok(())
}
