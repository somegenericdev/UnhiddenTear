```
 __    __            __        __        __        __                            ________                             
|  \  |  \          |  \      |  \      |  \      |  \                          |        \                            
| $$  | $$ _______  | $$____   \$$  ____| $$  ____| $$  ______   _______         \$$$$$$$$______    ______    ______  
| $$  | $$|       \ | $$    \ |  \ /      $$ /      $$ /      \ |       \          | $$  /      \  |      \  /      \ 
| $$  | $$| $$$$$$$\| $$$$$$$\| $$|  $$$$$$$|  $$$$$$$|  $$$$$$\| $$$$$$$\         | $$ |  $$$$$$\  \$$$$$$\|  $$$$$$\
| $$  | $$| $$  | $$| $$  | $$| $$| $$  | $$| $$  | $$| $$    $$| $$  | $$         | $$ | $$    $$ /      $$| $$   \$$
| $$__/ $$| $$  | $$| $$  | $$| $$| $$__| $$| $$__| $$| $$$$$$$$| $$  | $$         | $$ | $$$$$$$$|  $$$$$$$| $$      
 \$$    $$| $$  | $$| $$  | $$| $$ \$$    $$ \$$    $$ \$$     \| $$  | $$         | $$  \$$     \ \$$    $$| $$      
  \$$$$$$  \$$   \$$ \$$   \$$ \$$  \$$$$$$$  \$$$$$$$  \$$$$$$$ \$$   \$$          \$$   \$$$$$$$  \$$$$$$$ \$$      
                                                                                                                      
```

Unhidden Tear is a ransomware-like file crypter based on [Hidden Tear](https://github.com/goliate/hidden-tear).

## Features
The author of the original project had done a very poor work, both because he tried to backdoor it in order to prevent script kiddies from doing too much actual damage to other people and because of genuinely poor design choices.

This project aims to fix Hidden Tear's shortcomings in order to give people a chance to study how an actual ransomware works. 

Among the improvements are:

* Deletion of Microsoft Windows' [shadow copies](https://en.wikipedia.org/wiki/Shadow_Copy) in a similar fashion to many ransomwares such as [Cerber](https://malwiki.org/index.php?title=Cerber), [Locky](https://en.wikipedia.org/wiki/Locky) and [Powerware](https://www.microsoft.com/en-us/wdsi/threats/malware-encyclopedia-description?name=RANSOM:POWERSHELL/POWERWARE.A)
* Usage of [Argon2](https://en.wikipedia.org/wiki/Argon2), a strong, GPU-resistant, modern key derivation function, as opposed to PBKDF2
* The original project uses .NET's insecure Random class to generate the password as a way to make it easy to bruteforce for a victim. It was replaced with a Cryptographically Secure Pseudorandom Number Generator.
* Randomization of the key's salt; randomization and regeneration at every iteration (i.e. file) of AES' initialization vector (the original project uses a static salt and derives the IV from the key. This was too meant as a "backdoor" to help victims)
* Usage of HTTPS to send the key back to the Command and Control server

## TODO
Write decrypter.