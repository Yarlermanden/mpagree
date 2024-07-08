mod pb {
	tonic::include_proto!("mod");
}

pub mod engine {
	pub use crate::pb::engine::*;
}

pub mod grpc;

pub mod model;
