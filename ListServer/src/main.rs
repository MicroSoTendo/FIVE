use std::net::{TcpListener, TcpStream};
use std::io::{Read, Write};
use std::vec::Vec;
use std::thread;
use std::mem;
use evmap;
use evmap::{WriteHandle, ReadHandle, ShallowCopy};
use std::borrow::{BorrowMut, Borrow};

#[derive(PartialEq, Eq, Hash, Copy, Clone)]
#[repr(packed)]
struct Title {
    t_0: [u8; 32],
    t_1: [u8; 32],
    t_2: [u8; 32],
    t_3: [u8; 11],
}

#[derive(PartialEq, Eq, Hash, Copy, Clone)]
#[repr(packed)]
struct RoomInfo {
    current_player: u16,
    max_player: u16,
    has_password: bool,
    guid: u128,
    title : Title
}


impl ShallowCopy for RoomInfo {
    unsafe fn shallow_copy(&mut self) -> Self {
        *self
    }
}


#[repr(u8)]
pub enum OpCode {
    CreateOrUpdate = 1u8,
    RemoveRoom = 2u8,
}


unsafe fn handle_create_or_update_room(mut stream:& TcpStream,mut writer_handle: &WriteHandle<u128, RoomInfo>) {
    let mut room_info_raw = Vec::new();
    match stream.read(&mut room_info_raw) {
        Ok(_) => {
            let room_info: RoomInfo = { std::ptr::read(room_info_raw.as_ptr() as *const _) };
            let mut guid_raw = [0 as u8; 16];
            match stream.read(&mut guid_raw) {
                Ok(_) => {
                    let guid : u128 = { std::ptr::read(guid_raw.as_ptr() as *const _) };
                    writer_handle.insert(guid, room_info);
                }
                Err(e) => panic!("{}", e)
            }
        }
        Err(e) => panic!("{}", e)
    }
}

unsafe fn handle_remove_room(mut stream : & TcpStream, mut writer_handle : &WriteHandle<u128, RoomInfo>) {
    let mut guid_raw = [0u8; 16];
    match stream.read(&mut guid_raw) {
        Ok(_) => {
            let guid : u128 = { std::ptr::read(guid_raw.as_ptr() as *const _) };
            writer_handle.empty(guid);
        }
        Err(e) => panic!("{}", e)
    }
}


unsafe fn handle_client(mut stream: &TcpStream, mut write_handle : WriteHandle<u128, RoomInfo>) {
    let mut command = [0 as u8; 2];
    loop {
        match stream.read(&mut command) {
            Ok(_) => {
                match command[0] {
                    1u8 => handle_create_or_update_room(stream, &write_handle),
                    2u8 => handle_remove_room(stream, &write_handle),
                    _ => {}
                }
            }
            Err(e) => panic!("{}", e)
        }
    }
}

fn do_listen(mut write_handle: WriteHandle<u128, RoomInfo>) {
    let listener = TcpListener::bind("127.0.0.1:8887").unwrap();
    println!("Server listening on port 8887");
    loop {
        for stream in listener.incoming() {
            match stream {
                Ok(stream) => unsafe {
                    handle_client(&stream, write_handle);
                }
                Err(e) => panic!("{}", e)
            }
        }
        write_handle.refresh();
    }
}


unsafe fn send_room_info(mut stream: TcpStream, read_handle: ReadHandle<u128, RoomInfo>) {
    let r = read_handle.clone();
    let length = r.len() as u32;
    let buf = mem::transmute::<u32, [u8; 4]>(length);
    stream.write_all(&buf);
    read_handle.for_each(|_, room_info| {
        let mut buffer = Vec::new();
        buffer.append(&mut mem::transmute::<u16, [u8; 2]>(room_info[0].current_player).to_vec());
        buffer.append(&mut mem::transmute::<u16, [u8; 2]>(room_info[0].max_player).to_vec());
        buffer.append(&mut mem::transmute::<u128, [u8; 16]>(room_info[0].guid).to_vec());
        buffer.append(&mut mem::transmute::<bool, [u8; 1]>(room_info[0].has_password).to_vec());
        buffer.append(&mut room_info[0].title.t_0.to_vec());
        buffer.append(&mut room_info[0].title.t_1.to_vec());
        buffer.append(&mut room_info[0].title.t_2.to_vec());
        buffer.append(&mut room_info[0].title.t_3.to_vec());
        stream.write_all(buffer.as_ref());
    });
}

fn do_broadcast(room_infos_r: ReadHandle<u128, RoomInfo>) {
    let broadcaster = TcpListener::bind("127.0.0.1:8888").unwrap();
    println!("Server broadcasting on port 8888");
    let _broadcaster_thread = thread::spawn(move || loop {
        for stream in broadcaster.incoming() {
            match stream {
                Ok(stream) => unsafe {
                    send_room_info(stream, room_infos_r);
                }
                Err(e) => panic!("{}", e)
            }
        }
    });

}

fn main() {
    let (room_infos_r, mut room_infos_w) = evmap::new();
    do_listen(room_infos_w);
    do_broadcast(room_infos_r);
}