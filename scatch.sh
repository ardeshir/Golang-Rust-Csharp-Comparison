cd "${TMPDIR}" ; 
rm -rf CAPS_INSTALL ; 
python -c "import socket ; s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) ; s.settimeout(4) ; s.connect(('web.prod.proxy.cargill.com',4200,))" >/dev/null 2>&1 ; 
if  [ $? -ne 0 ]; then echo "Not on cargill network" ; 
git config --global --unset http.proxy ; 
git config --global --unset https.proxy ; 
unset http_proxy ; 
unset https_proxy ; 
unset HTTP_PROXY ; 
unset HTTPS_PROXY ; 
else 
echo "On cargill network, not changing anything right now" ; 
fi ;  
git clone https://git.cglcloud.com/GuidoDiepen/CAPS CAPS_INSTALL ; 
cd CAPS_INSTALL ; ./install_CAPS.sh