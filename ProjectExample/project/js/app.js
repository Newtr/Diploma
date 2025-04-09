class EvacuationPlan {
    constructor() {
        this.blockedCorridors = new Set();
        this.blockMode = false;
        this.currentPath = null;
        this.selectedRoom = null;
        this.mapElements = {};
        this.graph = {};
        
        this.roomConfig = roomConfig;
        this.priorityWeights = priorityWeights;

        this.initElements();
        this.drawBuilding();
        this.initControls();
        this.buildGraph();
    }

    initElements() {
        const elements = ['blockModeBtn', 'clearBlockedBtn', 'findPathBtn', 'clearPathBtn', 'infoPanel', 'mapContainer'];
        this.elements = elements.reduce((acc, id) => {
            acc[id] = document.getElementById(id);
            return acc;
        }, {});
    }

    drawBuilding() {
        this.elements.mapContainer.innerHTML = '';

        // Внешние стены
        this.createWall(0, 0, 1063, 10);
        this.createWall(0, 590, 1063, 10);
        this.createWall(0, 10, 10, 580);
        this.createWall(1053, 10, 10, 580);

        // Создаем элементы здания
        Object.entries(this.roomConfig).forEach(([id, config]) => {
            this.createBuildingElement(id, config);
        });
    }

    createBuildingElement(id, config) {
        const { type } = config;
        
        if (type === 'room') {
            this.createRoom(id, config);
        } else if (type === 'corridor' || type === 'cross') {
            this.createCorridor(id, config);
        } else if (type === 'stairs') {
            this.createStairs(id, config);
        } else if (type === 'exit') {
            this.createExit(id, config);
        }
    }

    createWall(x, y, width, height) {
        const wall = document.createElement('div');
        Object.assign(wall.style, {
            left: `${x}px`,
            top: `${y}px`,
            width: `${width}px`,
            height: `${height}px`
        });
        wall.className = 'wall';
        this.elements.mapContainer.appendChild(wall);
    }

    createRoom(id, {x, y, width, height, name}) {
        const room = this.createElementWithLabel(id, x, y, width, height, 'room', name);
        this.mapElements[id] = room.element;
        
        // Упрощенные стены комнаты
        this.createWall(x, y, width, 2);
        this.createWall(x + width - 2, y, 2, height);
        this.createWall(x, y + height - 2, width, 2);
        this.createWall(x, y, 2, height);
    }

    createCorridor(id, {x, y, width, height, name, type}) {
        const corridor = this.createElementWithLabel(
            id, x, y, width, height, 
            type === 'cross' ? 'cross' : 'corridor', 
            name
        );
        this.mapElements[id] = corridor.element;
    }

    createStairs(id, {x, y, width, height, name}) {
        const stairs = this.createElementWithLabel(id, x, y, width, height, 'stairs', name);
        this.mapElements[id] = stairs.element;

        // Рисуем ступени
        const stepCount = 5;
        const stepWidth = width / stepCount;
        for (let i = 0; i < stepCount; i++) {
            const step = document.createElement('div');
            Object.assign(step.style, {
                left: `${x + i * stepWidth}px`,
                top: `${y + i * 2}px`,
                width: `${stepWidth}px`,
                height: '5px'
            });
            step.className = 'stairs-step';
            this.elements.mapContainer.appendChild(step);
        }
    }

    createExit(id, {x, y, width, height, name}) {
        const exit = this.createElementWithLabel(id, x, y, width, height, 'exit', name);
        this.mapElements[id] = exit.element;
    }

    createElementWithLabel(id, x, y, width, height, className, name) {
        const element = document.createElement('div');
        Object.assign(element.style, {
            left: `${x}px`,
            top: `${y}px`,
            width: `${width}px`,
            height: `${height}px`
        });
        element.className = className;
        element.dataset.id = id;
        this.elements.mapContainer.appendChild(element);
        
        if (name) {
            const label = document.createElement('div');
            Object.assign(label.style, {
                left: `${x + 5}px`,
                top: `${y + 5}px`
            });
            label.className = `${className}-label`;
            label.textContent = name;
            this.elements.mapContainer.appendChild(label);
        }
        
        return { element };
    }

    buildGraph() {
        // Инициализация графа
        this.graph = Object.keys(this.roomConfig).reduce((acc, id) => {
            acc[id] = { 
                connections: [],
                type: this.roomConfig[id].type
            };
            return acc;
        }, {});

        // Добавление соединений
        connections.forEach(([a, b]) => this.addConnection(a, b));
    }

    addConnection(room1, room2) {
        if (!this.graph[room1] || !this.graph[room2]) return;
        
        if (!this.graph[room1].connections.includes(room2)) {
            this.graph[room1].connections.push(room2);
        }
        if (!this.graph[room2].connections.includes(room1)) {
            this.graph[room2].connections.push(room1);
        }
    }

    findShortestPathToExit(startId) {
        const exits = ['exit1', 'exit2', 'exit3'];
        let shortestPath = null;
        let minLength = Infinity;
        
        for (const exitId of exits) {
            const path = this.findPath(startId, exitId);
            if (path && path.length < minLength) {
                minLength = path.length;
                shortestPath = path;
            }
        }
        
        return shortestPath;
    }

    findPath(startId, endId) {
        if (!this.graph[startId] || !this.graph[endId]) {
            console.error(`Invalid start or end: ${startId} -> ${endId}`);
            return null;
        }

        const queue = [{ 
            id: startId, 
            path: [startId], 
            cost: 0,
            priority: this.priorityWeights[this.graph[startId].type] || 4
        }];
        
        const visited = new Set([startId]);

        while (queue.length > 0) {
            queue.sort((a, b) => a.priority - b.priority || a.cost - b.cost);
            const current = queue.shift();

            if (current.id === endId) {
                return current.path;
            }

            for (const neighbor of this.graph[current.id].connections) {
                if (this.blockedCorridors.has(neighbor)) continue;
                
                if (!visited.has(neighbor)) {
                    visited.add(neighbor);
                    queue.push({
                        id: neighbor,
                        path: [...current.path, neighbor],
                        cost: current.cost + 1,
                        priority: this.priorityWeights[this.graph[neighbor].type] || 4
                    });
                }
            }
        }

        return null;
    }

    findAlternativePath(startId) {
        const exits = ['exit1', 'exit2', 'exit3'];
        const paths = [];
        
        for (const exitId of exits) {
            const path = this.findPath(startId, exitId);
            if (path) {
                paths.push(path);
            }
        }
        
        if (paths.length > 0) {
            return paths.reduce((shortest, current) => 
                current.length < shortest.length ? current : shortest
            );
        }
        
        return null;
    }
    
    showPath(path) {
        this.clearPath();
        if (!path || path.length < 2) return;
        
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.classList.add('path-overlay');
        Object.assign(svg.style, {
            width: '100%',
            height: '100%',
            position: 'absolute',
            top: '0',
            left: '0',
            pointerEvents: 'none'
        });
        this.elements.mapContainer.appendChild(svg);
    
        // Создаем маркер стрелки
        const defs = document.createElementNS("http://www.w3.org/2000/svg", "defs");
        const marker = document.createElementNS("http://www.w3.org/2000/svg", "marker");
        marker.setAttribute('id', 'arrowhead');
        marker.setAttribute('markerWidth', '10');
        marker.setAttribute('markerHeight', '7');
        marker.setAttribute('refX', '9');
        marker.setAttribute('refY', '3.5');
        marker.setAttribute('orient', 'auto');
        const polygon = document.createElementNS("http://www.w3.org/2000/svg", "polygon");
        polygon.setAttribute('points', '0 0, 10 3.5, 0 7');
        polygon.setAttribute('fill', '#ff0000');
        marker.appendChild(polygon);
        defs.appendChild(marker);
        svg.appendChild(defs);
    
        // Основная линия пути
        const pathLine = document.createElementNS("http://www.w3.org/2000/svg", "path");
        pathLine.setAttribute('d', this.buildPathData(path));
        pathLine.setAttribute('stroke', 'rgba(247, 37, 133, 0.7)');
        pathLine.setAttribute('stroke-width', '4');
        pathLine.setAttribute('fill', 'none');
        pathLine.setAttribute('stroke-dasharray', '10, 5');
        svg.appendChild(pathLine);
    
        // Точки пути
        this.createPathPoints(svg, path);
    
        // Добавляем стрелку в конце пути с анимацией
        if (path.length >= 2) {
            const lastSegmentStart = this.getCenter(path[path.length-2]);
            const lastSegmentEnd = this.getCenter(path[path.length-1]);
            
            if (lastSegmentStart && lastSegmentEnd) {
                const arrow = document.createElementNS("http://www.w3.org/2000/svg", "line");
                arrow.classList.add('end-arrow');
                arrow.setAttribute('x1', lastSegmentStart.x);
                arrow.setAttribute('y1', lastSegmentStart.y);
                arrow.setAttribute('x2', lastSegmentEnd.x);
                arrow.setAttribute('y2', lastSegmentEnd.y);
                svg.appendChild(arrow);
            }
        }
    }
   
    buildPathData(path) {
        return path.map((id, i) => {
            const center = this.getCenter(id);
            return `${i === 0 ? 'M' : 'L'} ${center?.x || 0} ${center?.y || 0}`;
        }).join(' ');
    }

    createPathArrows(svg, path) {
        for (let i = 0; i < path.length - 1; i++) {
            const start = this.getCenter(path[i]);
            const end = this.getCenter(path[i+1]);
            
            if (start && end) {
                const arrow = document.createElementNS("http://www.w3.org/2000/svg", "line");
                arrow.setAttribute('x1', start.x);
                arrow.setAttribute('y1', start.y);
                arrow.setAttribute('x2', start.x);
                arrow.setAttribute('y2', start.y);
                arrow.setAttribute('stroke', '#ff0000');
                arrow.setAttribute('stroke-width', '3');
                arrow.setAttribute('marker-end', 'url(#arrowhead)');
                
                const animateX = document.createElementNS("http://www.w3.org/2000/svg", "animate");
                animateX.setAttribute('attributeName', 'x2');
                animateX.setAttribute('from', start.x);
                animateX.setAttribute('to', end.x);
                animateX.setAttribute('dur', '1s');
                animateX.setAttribute('begin', `${i}s`);
                animateX.setAttribute('fill', 'freeze');
                arrow.appendChild(animateX);
                
                const animateY = document.createElementNS("http://www.w3.org/2000/svg", "animate");
                animateY.setAttribute('attributeName', 'y2');
                animateY.setAttribute('from', start.y);
                animateY.setAttribute('to', end.y);
                animateY.setAttribute('dur', '1s');
                animateY.setAttribute('begin', `${i}s`);
                animateY.setAttribute('fill', 'freeze');
                arrow.appendChild(animateY);
                
                svg.appendChild(arrow);
            }
        }
    }

    createPathPoints(svg, path) {
        path.forEach((id, index) => {
            const center = this.getCenter(id);
            if (!center) return;
            
            const circle = document.createElementNS("http://www.w3.org/2000/svg", "circle");
            circle.setAttribute('cx', center.x);
            circle.setAttribute('cy', center.y);
            circle.setAttribute('r', '5');
            
            if (index === 0) {
                circle.setAttribute('fill', '#00ff00');
            } else if (index === path.length-1) {
                circle.setAttribute('fill', '#0000ff');
            } else if (this.graph[id].type === 'cross') {
                circle.setAttribute('fill', '#ff00ff');
            } else {
                circle.setAttribute('fill', '#ff9900');
            }
            
            const animate = document.createElementNS("http://www.w3.org/2000/svg", "animate");
            animate.setAttribute('attributeName', 'opacity');
            animate.setAttribute('from', '0');
            animate.setAttribute('to', '1');
            animate.setAttribute('dur', '0.5s');
            animate.setAttribute('begin', `${index * 0.8}s`);
            circle.appendChild(animate);
            
            svg.appendChild(circle);
        });
    }

    getCenter(elementId) {
        if (!this.elements?.mapContainer) return null;
        const el = this.mapElements[elementId];
        if (!el) return null;
        
        const rect = el.getBoundingClientRect();
        const containerRect = this.elements.mapContainer.getBoundingClientRect();
        
        return {
            x: rect.left + rect.width/2 - containerRect.left,
            y: rect.top + rect.height/2 - containerRect.top
        };
    }

    clearPath() {
        const paths = this.elements.mapContainer.querySelectorAll('.path-overlay');
        paths.forEach(p => p.remove());
    }

    initControls() {
        this.elements.blockModeBtn.addEventListener('click', () => this.toggleBlockMode());
        this.elements.clearBlockedBtn.addEventListener('click', () => this.clearBlocked());
        this.elements.findPathBtn.addEventListener('click', () => this.handleFindPath());
        this.elements.clearPathBtn.addEventListener('click', () => this.clearPath());
        
        this.elements.mapContainer.addEventListener('click', (e) => this.handleMapClick(e));
    }

    toggleBlockMode() {
        this.blockMode = !this.blockMode;
        this.elements.blockModeBtn.classList.toggle('active');
        this.elements.blockModeBtn.textContent = this.blockMode ? 
            'Режим блокировки (активен)' : 
            'Режим блокировки';
        
        if (this.blockMode && this.selectedRoom) {
            this.findAndShowPath(this.selectedRoom);
        }
    }
    
    clearBlocked() {
        this.blockedCorridors.forEach(id => {
            this.mapElements[id]?.classList.remove('blocked');
        });
        this.blockedCorridors.clear();
        
        if (this.selectedRoom) {
            this.findAndShowPath(this.selectedRoom);
        }
    }
    
    handleFindPath() {
        if (this.selectedRoom) {
            this.findAndShowPath(this.selectedRoom);
        } else {
            this.showAlert('Пожалуйста, сначала выберите комнату');
        }
    }
    
    handleMapClick(e) {
        const element = e.target.closest('.room, .corridor, .cross, .exit');
        if (!element) return;
        
        const id = element.dataset.id;
        
        if (this.blockMode && (element.classList.contains('corridor') || element.classList.contains('cross'))) {
            this.toggleCorridorBlock(id, element);
            if (this.selectedRoom) {
                this.findAndShowPath(this.selectedRoom);
            }
        } else if (element.classList.contains('room')) {
            this.selectRoom(id, element);
        } else if (element.classList.contains('exit')) {
            this.selectExit(id, element);
        }
    }

    toggleCorridorBlock(id, element) {
        if (this.blockedCorridors.has(id)) {
            this.blockedCorridors.delete(id);
            element.classList.remove('blocked');
        } else {
            this.blockedCorridors.add(id);
            element.classList.add('blocked');
        }
    }

    selectRoom(id, element) {
        if (this.blockMode) return;
        
        if (this.selectedRoom) {
            this.mapElements[this.selectedRoom]?.classList.remove('selected');
        }
        
        this.selectedRoom = id;
        element.classList.add('selected');
        this.showAlert(`Выбрана комната: ${this.roomConfig[id].name}`);
        this.findAndShowPath(id);
    }

    selectExit(id, element) {
        if (this.selectedRoom) {
            this.findAndShowPath(this.selectedRoom, id);
        }
    }

    findAndShowPath(startRoomId, exitId = null) {
        let path;
        
        if (exitId) {
            path = this.findPath(startRoomId, exitId);
        } else {
            path = this.findAlternativePath(startRoomId);
        }
            
        if (path) {
            this.currentPath = path;
            this.showPath(path);
            this.showAlert(`Построен путь до ${exitId ? this.roomConfig[exitId].name : 'ближайшего выхода'}`);
        } else {
            this.clearPath();
            this.showAlert('Путь не найден! Все возможные маршруты заблокированы');
        }
    }

    showAlert(message) {
        this.elements.infoPanel.textContent = message;
        this.elements.infoPanel.classList.add('show');
        setTimeout(() => {
            this.elements.infoPanel.classList.remove('show');
        }, 3000);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new EvacuationPlan();
});