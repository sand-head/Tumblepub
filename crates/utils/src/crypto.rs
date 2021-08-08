use anyhow::Result;
use base64::decode;
use openssl::{pkey::PKey, rsa::Rsa};

pub struct KeyPair {
  pub private_key: Vec<u8>,
  pub public_key: Vec<u8>,
}
impl KeyPair {
  pub fn generate() -> Result<Self> {
    let rsa_pair = Rsa::generate(2048)?;
    let public_key = PKey::from_rsa(rsa_pair)?;

    Ok(KeyPair {
      private_key: public_key.private_key_to_pem_pkcs8()?,
      public_key: public_key.public_key_to_pem()?,
    })
  }
}

pub struct Signature {
  pub key_id: String,
  pub headers: Vec<String>,
  pub signature: Vec<u8>,
}
impl Signature {
  pub fn parse(value: &str) -> Result<Self> {
    // this is probably not a good way to parse this
    let mut key_id = "".to_owned();
    let mut headers = "".to_owned();
    let mut signature = vec![];

    for section in value.split(',').into_iter() {
      let key_value_pair: Vec<&str> = section.split('=').into_iter().collect();
      let value = key_value_pair[1];

      match key_value_pair[0] {
        "keyId" => key_id = value[1..value.len() - 1].to_owned(),
        "headers" => headers = value[1..value.len() - 1].to_owned(),
        "signature" => signature = decode(&value[1..value.len() - 1]).unwrap_or_default(),
        _ => (),
      }
    }

    Ok(Self {
      key_id,
      headers: headers.split(' ').map(|h| h.to_owned()).collect(),
      signature,
    })
  }
}
