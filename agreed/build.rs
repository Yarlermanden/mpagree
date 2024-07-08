fn main() -> Result<(), Box<dyn std::error::Error>> {
	tonic_build::configure().include_file("mod.rs").compile(&["../protos/mpagree/engine/service.proto"], &["../protos"])?;
	Ok(())
}
