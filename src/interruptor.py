import os

def main():
    my_path = "./../test/source/wallpapers/veryimportantfile.txt"
    while True:
        if (os.path.exists(my_path)):
            change_hash(my_path)
        else:
            break
    
def change_hash(my_path):
    with open (my_path, "a") as f:
        f.write("hey")

if __name__ == "__main__":
    main()