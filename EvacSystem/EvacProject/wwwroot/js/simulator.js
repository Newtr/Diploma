const roomConfig = {
    'room1': {x: 12, y: 48, width: 165, height: 79, name: '1', type: 'room'},
    'room2': {x: 214, y: 48, width: 120, height: 115, name: '2', type: 'room'},
    'room3': {x: 12, y: 164, width: 165, height: 110, name: '3', type: 'room'},
    'room4': {x: 214, y: 164, width: 120, height: 110, name: '4', type: 'room'},
    'room5': {x: 12, y: 275, width: 165, height: 94, name: '5', type: 'room'},
    'room6': {x: 214, y: 275, width: 120, height: 94, name: '6', type: 'room'},
    'room7A': {x: 12, y: 370, width: 165, height: 90, name: '7A', type: 'room'},
    'room7B': {x: 12, y: 461, width: 165, height: 128, name: '7B', type: 'room'},
    'room8': {x: 214, y: 370, width: 120, height: 90, name: '8', type: 'room'},
    'room9': {x: 214, y: 497, width: 120, height: 92, name: '9', type: 'room'},
    'room10': {x: 335, y: 388, width: 120, height: 72, name: '10', type: 'room'},
    'room11': {x: 335, y: 497, width: 120, height: 92, name: '11', type: 'room'},
    'room12': {x: 456, y: 388, width: 120, height: 72, name: '12', type: 'room'},
    'room13': {x: 613, y: 497, width: 140, height: 92, name: '13', type: 'room'},
    'room14': {x: 613, y: 388, width: 140, height: 72, name: '14', type: 'room'},
    'room15': {x: 754, y: 497, width: 140, height: 92, name: '15', type: 'room'},
    'room16': {x: 754, y: 388, width: 140, height: 72, name: '16', type: 'room'},
    'room17': {x: 895, y: 497, width: 121, height: 92, name: '17', type: 'room'},
    'room18': {x: 895, y: 361, width: 121, height: 99, name: '18', type: 'room'},
    'room19': {x: 895, y: 250, width: 121, height: 110, name: '19', type: 'room'},
    'room20': {x: 895, y: 149, width: 121, height: 100, name: '20', type: 'room'},
    'room21': {x: 895, y: 48, width: 121, height: 100, name: '21', type: 'room'},
    'room22': {x: 754, y: 48, width: 140, height: 110, name: '22', type: 'room'},
    'room23': {x: 613, y: 48, width: 140, height: 110, name: '23', type: 'room'},
    'room24': {x: 613, y: 159, width: 281, height: 110, name: '24', type: 'room'},
    'room25': {x: 613, y: 270, width: 281, height: 117, name: '25', type: 'room'},
    'room26': {x: 335, y: 270, width: 241, height: 117, name: '26', type: 'room'},
    'room27': {x: 335, y: 159, width: 241, height: 110, name: '27', type: 'room'},
    'room28': {x: 456, y: 48, width: 120, height: 110, name: '28', type: 'room'},
    'room29': {x: 335, y: 48, width: 120, height: 110, name: '29', type: 'room'},
    'corridor2_1': {x: 613, y: 461, width: 140, height: 35, name: 'К2-1', type: 'corridor'},
    'corridor2_2': {x: 754, y: 461, width: 140, height: 35, name: 'К2-2', type: 'corridor'},
    'corridor2_3': {x: 895, y: 461, width: 121, height: 35, name: 'К2-3', type: 'corridor'},
    'corridor3_1': {x: 1017, y: 48, width: 35, height: 100, name: 'К3-1', type: 'corridor'},
    'corridor3_2': {x: 1017, y: 149, width: 35, height: 100, name: 'К3-2', type: 'corridor'},
    'corridor3_3': {x: 1017, y: 250, width: 35, height: 110, name: 'К3-3', type: 'corridor'},
    'corridor3_4': {x: 1017, y: 361, width: 35, height: 99, name: 'К3-4', type: 'corridor'},
    'corridor4_1': {x: 613, y: 12, width: 140, height: 35, name: 'К4-1', type: 'corridor'},
    'corridor4_2': {x: 754, y: 12, width: 140, height: 35, name: 'К4-2', type: 'corridor'},
    'corridor4_3': {x: 895, y: 12, width: 121, height: 35, name: 'К4-3', type: 'corridor'},
    'corridor5_1': {x: 577, y: 48, width: 35, height: 110, name: 'К5-1', type: 'corridor'},
    'corridor5_2': {x: 577, y: 159, width: 35, height: 110, name: 'К5-2', type: 'corridor'},
    'corridor5_3': {x: 577, y: 270, width: 35, height: 117, name: 'К5-3', type: 'corridor'},
    'corridor6_1': {x: 456, y: 461, width: 120, height: 35, name: 'К6-1', type: 'corridor'},
    'corridor6_2': {x: 577, y: 388, width: 35, height: 72, name: 'К6-2', type: 'corridor'},
    'corridor6_3': {x: 456, y: 497, width: 120, height: 50, name: 'К6-3', type: 'corridor'},
    'corridor7_1': {x: 214, y: 12, width: 120, height: 35, name: 'К7-1', type: 'corridor'},
    'corridor7_2': {x: 335, y: 12, width: 120, height: 35, name: 'К7-2', type: 'corridor'},
    'corridor7_3': {x: 456, y: 12, width: 120, height: 35, name: 'К7-3', type: 'corridor'},
    'corridor8': {x: 178, y: 48, width: 35, height: 79, name: 'К8', type: 'corridor'},
    'corridor9_1': {x: 71, y: 128, width: 52, height: 35, name: 'К9-1', type: 'corridor'},
    'corridor9_2': {x: 124, y: 128, width: 52, height: 35, name: 'К9-2', type: 'corridor'},
    'corridor10_1': {x: 178, y: 164, width: 35, height: 110, name: 'К10-1', type: 'corridor'},
    'corridor10_2': {x: 178, y: 275, width: 35, height: 94, name: 'К10-2', type: 'corridor'},
    'corridor10_3': {x: 178, y: 370, width: 35, height: 90, name: 'К10-3', type: 'corridor'},
    'corridor11_1': {x: 214, y: 461, width: 120, height: 35, name: 'К11-1', type: 'corridor'},
    'corridor11_2': {x: 335, y: 461, width: 120, height: 35, name: 'К11-2', type: 'corridor'},
    'cross1': {x: 1017, y: 461, width: 35, height: 35, name: 'c1', type: 'cross'},
    'cross2': {x: 178, y: 461, width: 35, height: 35, name: 'c2', type: 'cross'},
    'cross3': {x: 178, y: 128, width: 35, height: 35, name: 'c3', type: 'cross'},
    'cross4': {x: 178, y: 12, width: 35, height: 35, name: 'c4', type: 'cross'},
    'cross5': {x: 577, y: 12, width: 35, height: 35, name: 'c5', type: 'cross'},
    'cross6': {x: 577, y: 461, width: 35, height: 35, name: 'c6', type: 'cross'},
    'exit1': {x: 456, y: 548, width: 120, height: 42, name: 'Выход 1', type: 'exit'},
    'exit2': {x: 0, y: 128, width: 70, height: 35, name: 'Выход 2', type: 'exit'},
    'exit3': {x: 1017, y: 10, width: 70, height: 35, name: 'Выход 3', type: 'exit'}
};

const priorityWeights = {
    'exit': 1,
    'cross': 2,
    'corridor': 3,
    'stairs': 3,
    'room': 4
};

const connections = [
    ['room1', 'corridor8'], ['room2', 'corridor7_1'], ['room3', 'corridor10_1'],
    ['room4', 'corridor10_1'], ['room5', 'corridor10_2'], ['room6', 'corridor10_2'],
    ['room7A', 'corridor10_3'], ['room7A', 'room7B'], ['room8', 'corridor10_3'],
    ['room9', 'corridor11_1'], ['room10', 'corridor11_2'], ['room11', 'corridor4_1'],
    ['room12', 'corridor6_1'], ['room13', 'corridor2_1'], ['room14', 'corridor2_1'],
    ['room15', 'corridor2_2'], ['room16', 'corridor2_2'], ['room17', 'corridor2_3'],
    ['room18', 'corridor3_4'], ['room19', 'corridor3_3'], ['room20', 'corridor3_2'],
    ['room21', 'corridor4_3'], ['room22', 'corridor4_2'], ['room23', 'corridor4_1'],
    ['room24', 'corridor5_2'], ['room25', 'corridor5_3'], ['room26', 'corridor5_3'],
    ['room27', 'corridor5_2'], ['room28', 'corridor7_3'], ['room29', 'corridor7_2'],
    ['corridor9_1', 'corridor9_2'], ['corridor10_1', 'corridor10_2'],
    ['corridor2_1', 'corridor2_2'], ['corridor2_2', 'corridor2_3'],
    ['corridor2_3', 'cross1'], ['corridor3_1', 'corridor3_2'],
    ['corridor3_2', 'corridor3_3'], ['corridor3_3', 'corridor3_4'],
    ['corridor3_4', 'cross1'], ['corridor4_1', 'corridor4_2'],
    ['corridor4_2', 'corridor4_3'], ['corridor4_3', 'exit3'],
    ['corridor5_1', 'corridor5_2'], ['corridor5_2', 'corridor5_3'],
    ['corridor5_3', 'corridor6_2'], ['corridor6_1', 'corridor6_3'],
    ['corridor7_1', 'corridor7_2'], ['corridor7_2', 'corridor7_3'],
    ['corridor7_3', 'cross5'], ['corridor9_1', 'corridor9_2'],
    ['corridor9_1', 'exit2'], ['corridor10_1', 'corridor10_2'],
    ['corridor10_2', 'corridor10_3'], ['corridor11_1', 'corridor11_2'],
    ['corridor11_2', 'corridor6_1'], ['cross1', 'corridor3_4'],
    ['cross1', 'corridor2_3'], ['cross2', 'corridor10_3'],
    ['cross2', 'corridor11_1'], ['cross3', 'corridor8'],
    ['cross3', 'corridor9_2'], ['cross3', 'corridor10_1'],
    ['cross4', 'corridor7_1'], ['cross4', 'corridor8'],
    ['cross5', 'corridor4_1'], ['cross5', 'corridor5_1'],
    ['cross6', 'corridor6_2'], ['cross6', 'corridor6_1'],
    ['cross6', 'corridor2_1'], ['corridor6_3', 'exit1'],
    ['corridor9_1', 'exit2'], ['corridor4_3', 'exit3'],
    ['corridor3_1', 'exit3']
];

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
        this.createWall(0, 0, 1063, 10);
        this.createWall(0, 590, 1063, 10);
        this.createWall(0, 10, 10, 580);
        this.createWall(1053, 10, 10, 580);

        Object.entries(this.roomConfig).forEach(([id, config]) => {
            this.createBuildingElement(id, config);
        });
    }

    createBuildingElement(id, config) {
        const { type } = config;
        if (type === 'room') this.createRoom(id, config);
        else if (type === 'corridor' || type === 'cross') this.createCorridor(id, config);
        else if (type === 'stairs') this.createStairs(id, config);
        else if (type === 'exit') this.createExit(id, config);
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
        this.createWall(x, y, width, 2);
        this.createWall(x + width - 2, y, 2, height);
        this.createWall(x, y + height - 2, width, 2);
        this.createWall(x, y, 2, height);
    }

    createCorridor(id, {x, y, width, height, name, type}) {
        const corridor = this.createElementWithLabel(id, x, y, width, height, type === 'cross' ? 'cross' : 'corridor', name);
        this.mapElements[id] = corridor.element;
    }

    createStairs(id, {x, y, width, height, name}) {
        const stairs = this.createElementWithLabel(id, x, y, width, height, 'stairs', name);
        this.mapElements[id] = stairs.element;
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
        this.graph = Object.keys(this.roomConfig).reduce((acc, id) => {
            acc[id] = { connections: [], type: this.roomConfig[id].type };
            return acc;
        }, {});
        connections.forEach(([a, b]) => this.addConnection(a, b));
    }

    addConnection(room1, room2) {
        if (!this.graph[room1] || !this.graph[room2]) return;
        if (!this.graph[room1].connections.includes(room2)) this.graph[room1].connections.push(room2);
        if (!this.graph[room2].connections.includes(room1)) this.graph[room2].connections.push(room1);
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
        if (!this.graph[startId] || !this.graph[endId]) return null;
        const queue = [{ id: startId, path: [startId], cost: 0, priority: this.priorityWeights[this.graph[startId].type] || 4 }];
        const visited = new Set([startId]);
        while (queue.length > 0) {
            queue.sort((a, b) => a.priority - b.priority || a.cost - b.cost);
            const current = queue.shift();
            if (current.id === endId) return current.path;
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
            if (path) paths.push(path);
        }
        if (paths.length > 0) {
            return paths.reduce((shortest, current) => current.length < shortest.length ? current : shortest, paths[0]);
        }
        return null;
    }

    showPath(path) {
        this.clearPath();
        if (!path || path.length < 2) return;
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.classList.add('path-overlay');
        Object.assign(svg.style, { width: '100%', height: '100%', position: 'absolute', top: '0', left: '0', pointerEvents: 'none' });
        this.elements.mapContainer.appendChild(svg);
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
        marker.appendChild(polygon);
        defs.appendChild(marker);
        svg.appendChild(defs);
        const pathLine = document.createElementNS("http://www.w3.org/2000/svg", "path");
        pathLine.setAttribute('d', this.buildPathData(path));
        svg.appendChild(pathLine);
        this.createPathPoints(svg, path);
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

    createPathPoints(svg, path) {
        path.forEach((id, index) => {
            const center = this.getCenter(id);
            if (!center) return;
            const circle = document.createElementNS("http://www.w3.org/2000/svg", "circle");
            circle.setAttribute('cx', center.x);
            circle.setAttribute('cy', center.y);
            if (index === 0) circle.classList.add('start-point');
            else if (index === path.length-1) circle.classList.add('end-point');
            else if (this.graph[id].type === 'cross') circle.classList.add('cross-point');
            else circle.classList.add('mid-point');
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
        this.elements.blockModeBtn.textContent = this.blockMode ? 'Режим блокировки (активен)' : 'Режим блокировки';
        if (this.blockMode && this.selectedRoom) this.findAndShowPath(this.selectedRoom);
    }

    clearBlocked() {
        this.blockedCorridors.forEach(id => this.mapElements[id]?.classList.remove());
        blocked 
        this.blockedCorridors.clear();
        if (this.selectedRoom) this.findAndShow(this.selectedRoomId);
    }

    handleFindPath() {
        if (this.selectedRoom) this.findAndShowPath(this.selectedRoom);
        else this.showAlert('Пожалуйста, выберите комнату сначала');
    }

    handleMapClick(e) {
        const element = e.target.closest('.room, .corridor, .cross, .exit');
        if (!element) return;
        const id = element.dataset.id;
        if (this.blockMode && (element.classList.contains('corridor') || element.classList.contains('cross')) ){
            this.toggleCorridorBlock(id, element);
            if (this.selectedRoom) this.findAndShowPath(this.selectedRoom);
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
        if (this.selectedRoom) this.mapElements[this.selectedRoom]?.classList.remove('selected');
        this.selectedRoom = id;
        element.classList.add('selected');
        this.showAlert(`Выбрана комната: ${this.roomConfig[id]?.name || id}`);
        this.findAndShowPath(id);
    }

    selectExit(id, element) {
        if (this.selectedRoom) this.findAndShowPath(this.selectedRoom, id);
    }

    findAndShowPath(startRoomId, exitId = null) {
        const path = exitId ? this.findPath(startRoomId, exitId) : this.findAlternativePath(startRoomId);
        if (path) {
            this.currentPath = path;
            this.showPath(path);
            this.showAlert(`Построен путь до ${exitId ? this.roomConfig[exitId]?.name || exitId : 'ближайшего выхода'}`);
        } else {
            this.clearPath();
            this.showAlert('Путь не найден! Все маршруты заблокированны');
        }
    }

    showAlert(message) {
        this.elements.infoPanel.textContent = message;
        this.elements.infoPanel.classList.add('show');
        setTimeout(() => this.elements.infoPanel.classList.remove('show'), 3000);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new EvacuationPlan();
    document.querySelectorAll('.nav-button').forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            document.body.classList.add('fade-out');
            setTimeout(() => {
                window.location.href = this.href;
            }, 500);
        });
    });
});