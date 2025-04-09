// Конфигурация помещений
const roomConfig = {
    // Кабинеты (1-25)
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
    'room10':{x: 335, y: 388, width: 120, height: 72, name: '10', type:'room'},
    'room11':{x: 335, y: 497, width: 120, height: 92, name: '11', type:'room'},
    'room12':{x: 456, y: 388, width: 120, height: 72, name: '12', type:'room'},
    'room13':{x: 613, y: 497, width: 140, height: 92, name: '13', type:'room'},
    'room14':{x: 613, y: 388, width: 140, height: 72, name: '14', type:'room'},
    'room15':{x: 754, y: 497, width: 140, height: 92, name: '15', type:'room'},
    'room16':{x: 754, y: 388, width: 140, height: 72, name: '16', type:'room'},
    'room17':{x: 895, y: 497, width: 121, height: 92, name: '17', type:'room'},
    'room18':{x: 895, y: 361, width: 121, height: 99, name: '18', type:'room'},
    'room19':{x: 895, y: 250, width: 121, height: 110, name: '19', type:'room'},
    'room20':{x: 895, y: 149, width: 121, height: 100, name: '20', type:'room'},
    'room21':{x: 895, y: 48, width: 121, height: 100, name: '21', type:'room'},
    'room22':{x: 754, y: 48, width: 140, height: 110, name: '22', type:'room'},
    'room23':{x: 613, y: 48, width: 140, height: 110, name: '23', type:'room'},
    'room24':{x: 613, y: 159, width: 281, height: 110, name: '24', type:'room'},
    'room25':{x: 613, y: 270, width: 281, height: 117, name: '25', type:'room'},
    'room26':{x: 335, y: 270, width: 241, height: 117, name: '26', type:'room'},
    'room27':{x: 335, y: 159, width: 241, height: 110, name: '27', type:'room'},
    'room28':{x: 456, y: 48, width: 120, height: 110, name: '28', type:'room'},
    'room29':{x: 335, y: 48, width: 120, height: 110, name: '29', type:'room'},

    /* Лестницы
    'stairs1': {x: 789, y: 381, width: 83, height: 84, name: 'Лестница', type: 'stairs'},
    */
    // Раздробленные коридоры
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
    
    // Перекрестки
    'cross1': {x: 1017, y: 461, width: 35, height: 35, name: 'c1', type: 'cross'},
    'cross2': {x: 178, y: 461, width: 35, height: 35, name: 'c2', type: 'cross'},
    'cross3': {x: 178, y: 128, width: 35, height: 35, name: 'c3', type: 'cross'},
    'cross4': {x: 178, y: 12, width: 35, height: 35, name: 'c4', type: 'cross'},
    'cross5': {x: 577, y: 12, width: 35, height: 35, name: 'c5', type: 'cross'},
    'cross6': {x: 577, y: 461, width: 35, height: 35, name: 'c6', type: 'cross'},
    //'cross7': {x: 594, y: 383, width: 57, height: 85, name: 'c7', type: 'cross'},
    
    // Выходы
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
    ['room7A', 'corridor10_3'],['room7A','room7B'],
    ['room7', 'corridor5_3'], ['room8', 'corridor10_3'], ['room9', 'corridor11_1'],
    ['room10', 'corridor11_2'], ['room11', 'corridor4_1'],
    ['room12', 'corridor6_1'], ['room13', 'corridor2_1'], ['room14', 'corridor2_1'],
    ['room15', 'corridor2_2'], ['room16', 'corridor2_2'], ['room17', 'corridor2_3'],
    ['room18', 'corridor3_4'], ['room19', 'corridor3_3'], ['room20', 'corridor3_2'],
    ['room21', 'corridor4_3'],['room22', 'corridor4_2'], ['room23', 'corridor4_1'],
    ['room24', 'corridor5_2'],['room25', 'corridor5_3'],['room26','corridor5_3'],
    ['room27','corridor5_2'],['room28', 'corridor7_3'],['room29', 'corridor7_2'],

    // Соединение коридоров
    ['corridor9_1','corridor9_2'],
    ['corridor10_1','corridor10_2'],
    // Комнаты первого этажа
    
    // Соединения коридоров
    ['corridor2_1', 'corridor2_2'], ['corridor2_2', 'corridor2_3'], 
    ['corridor2_3', 'cross1'],
    
    ['corridor3_1', 'corridor3_2'], ['corridor3_2', 'corridor3_3'], 
    ['corridor3_3', 'corridor3_4'], ['corridor3_4', 'cross1'],
    
    ['corridor4_1', 'corridor4_2'], ['corridor4_2', 'corridor4_3'],
    ['corridor4_3', 'exit3'],
    
    ['corridor5_1', 'corridor5_2'], ['corridor5_2', 'corridor5_3'], 
    ['corridor5_3', 'corridor6_2'], 
    
    ['corridor6_3', 'corridor6_4'],
    ['corridor6_1', 'corridor6_3'], ['corridor6_2', 'corridor6_4'],
    
    
    ['corridor7_1', 'corridor7_2'], ['corridor7_2', 'corridor7_3'],
    ['corridor7_3', 'cross5'],
    
    ['corridor9_1', 'corridor9_2'], ['corridor9_1', 'exit2'],
    
    ['corridor10_1', 'corridor10_2'], ['corridor10_2', 'corridor10_3'],
    
    ['corridor11_1', 'corridor11_2'],['corridor11_2','corridor6_1'],
    
    // Соединения лестницы
    //['stairs1', 'cross6'], ['cross6', 'corridor2_2'], 
     ['corridor6_4', 'corridor2_1'],
    
    
    ['cross1', 'corridor3_4'], ['cross1', 'corridor2_4'],
    ['cross2', 'corridor10_3'], ['cross2', 'corridor11_1'],
    ['cross3', 'corridor8'], ['cross3', 'corridor9_2'], ['cross3', 'corridor10_1'],
    ['cross4', 'corridor7_1'], ['cross4', 'corridor8'],
    ['cross5', 'corridor4_1'], ['cross5', 'corridor5_1'],
    ['cross6', 'corridor6_2'],['cross6','corridor6_1'],['cross6','corridor2_1'],
    
    
    // Соединения перекрестков
    ['cross4','corridor7_1'],['cross4','corrridor8'],
    ['cross3','corridor8'],['cross3','corridor9_2'],['cross3','corridor10_1'],
    // Выходы
    ['corridor6_3', 'exit1'], ['corridor9_1', 'exit2'], ['corridor4_3', 'exit3'],
    ['corridor6_4', 'exit1'],['corridor3_1','exit3'],
    
]
;