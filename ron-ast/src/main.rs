use std::{fs, path::Path};

use clap::Parser;
use ron::ser::{PrettyConfig, to_string_pretty as to_ron_string};
use ron2::ast::parse_document;
use serde_json::to_string_pretty as to_json_string;

#[derive(Parser, Debug)]
struct Config {
    file: String,
    output: Option<String>,
}

fn main() {
    let config = Config::parse();
    let source = fs::read_to_string(&config.file).unwrap();
    let ast = parse_document(&source).unwrap();
    let ron = to_ron_string(&ast, PrettyConfig::new().struct_names(true)).unwrap();
    let json = to_json_string(&ast).unwrap();
    match config.output {
        Some(output) => {
            let extension = Path::new(&output).extension().unwrap().to_str().unwrap();
            match extension {
                "ron" => {
                    fs::write(&output, &ron).unwrap();
                }
                "json" => {
                    fs::write(&output, &json).unwrap();
                }
                _ => {
                    panic!("Unknown format {}", &extension)
                }
            }
        }
        None => {
            println!("{}", &ron);
        }
    }
}
